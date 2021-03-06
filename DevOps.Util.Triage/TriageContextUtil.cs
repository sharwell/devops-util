#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevOps.Util;
using DevOps.Util.DotNet;

namespace DevOps.Util.Triage
{
    public enum IssueKind
    {
        Azure,
        Helix,

        NuGet,

        // General infrastructure owned by the .NET Team
        Infra,

        Build,
        Test,
        Other
    }

    public sealed class TriageContextUtil
    {
        public TriageContext Context { get; }

        public TriageContextUtil(TriageContext context)
        {
            Context = context;
        }

        public static string GetModelBuildId(BuildKey buildKey) => 
            $"{buildKey.Organization}-{buildKey.Project}-{buildKey.Number}";

        public static GitHubIssueKey GetGitHubIssueKey(ModelTimelineQuery timelineQuery) =>
            new GitHubIssueKey(timelineQuery.GitHubOrganization, timelineQuery.GitHubRepository, timelineQuery.IssueNumber);

        public static GitHubPullRequestKey? GetGitHubPullRequestKey(ModelBuild build) =>
            build.PullRequestNumber.HasValue
                ? (GitHubPullRequestKey?)new GitHubPullRequestKey(build.GitHubOrganization, build.GitHubRepository, build.PullRequestNumber.Value)
                : null;

        public static BuildDefinitionInfo GetBuildDefinitionInfo(ModelBuildDefinition buildDefinition) =>
            new BuildDefinitionInfo(
                buildDefinition.AzureOrganization,
                buildDefinition.AzureProject,
                buildDefinition.DefinitionName,
                buildDefinition.DefinitionId);

        public static BuildKey GetBuildKey(ModelBuild build) =>
            new BuildKey(build.ModelBuildDefinition.AzureOrganization, build.ModelBuildDefinition.AzureProject, build.BuildNumber);

        public static BuildInfo GetBuildInfo(ModelBuild build) =>
            new BuildInfo(
                GetBuildKey(build),
                GetBuildDefinitionInfo(build.ModelBuildDefinition),
                build.GitHubOrganization,
                build.GitHubRepository,
                build.PullRequestNumber,
                build.StartTime,
                build.FinishTime);

        /// <summary>
        /// Determine if this build has already been processed for this query
        /// </summary>
        public bool IsProcessed(ModelTimelineQuery timelineQuery, ModelBuild modelBuild) =>
            Context.ModelTimelineQueryCompletes.Any(x =>
                x.ModelTimelineQueryId == timelineQuery.Id &&
                x.ModelBuildId == modelBuild.Id);

        public ModelBuildDefinition EnsureBuildDefinition(BuildDefinitionInfo definitionInfo)
        {
            var buildDefinition = Context.ModelBuildDefinitions
                .Where(x =>
                    x.AzureOrganization == definitionInfo.Organization &&
                    x.AzureProject == definitionInfo.Project &&
                    x.DefinitionId == definitionInfo.Id)
                .FirstOrDefault();
            if (buildDefinition is object)
            {
                return buildDefinition;
            }

            buildDefinition = new ModelBuildDefinition()
            {
                AzureOrganization = definitionInfo.Organization,
                AzureProject = definitionInfo.Project,
                DefinitionId = definitionInfo.Id,
                DefinitionName = definitionInfo.Name,
            };

            Context.ModelBuildDefinitions.Add(buildDefinition);
            Context.SaveChanges();
            return buildDefinition;
        }

        public ModelBuild EnsureBuild(BuildInfo buildInfo)
        {
            var modelBuildId = GetModelBuildId(buildInfo.Key);
            var modelBuild = Context.ModelBuilds
                .Where(x => x.Id == modelBuildId)
                .FirstOrDefault();
            if (modelBuild is object)
            {
                return modelBuild;
            }

            var prKey = buildInfo.PullRequestKey;
            modelBuild = new ModelBuild()
            {
                Id = modelBuildId,
                ModelBuildDefinitionId = EnsureBuildDefinition(buildInfo.DefinitionInfo).Id,
                GitHubOrganization = prKey?.Organization,
                GitHubRepository = prKey?.Repository,
                PullRequestNumber = prKey?.Number,
                StartTime = buildInfo.StartTime,
                FinishTime = buildInfo.FinishTime,
                BuildNumber = buildInfo.Number,
            };
            Context.ModelBuilds.Add(modelBuild);
            Context.SaveChanges();
            return modelBuild;
        }

        public bool TryGetTimelineQuery(GitHubIssueKey issueKey, [NotNullWhen(true)] out ModelTimelineQuery timelineQuery)
        {
            timelineQuery = Context.ModelTimelineQueries
                .Where(x => 
                    x.GitHubOrganization == issueKey.Organization &&
                    x.GitHubRepository == issueKey.Repository &&
                    x.IssueNumber == issueKey.Number)
                .FirstOrDefault();
            return timelineQuery is object;
        }

        public bool TryCreateTimelineQuery(IssueKind kind, GitHubIssueKey issueKey, string text)
        {
            if (TryGetTimelineQuery(issueKey, out var timelineQuery))
            {
                return false;
            }

            timelineQuery = new ModelTimelineQuery()
            {
                GitHubOrganization = issueKey.Organization,
                GitHubRepository = issueKey.Repository,
                IssueNumber = issueKey.Number,
                SearchText = text
            };

            try
            {
                Context.ModelTimelineQueries.Add(timelineQuery);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}