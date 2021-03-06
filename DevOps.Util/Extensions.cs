#nullable enable

using System;

namespace DevOps.Util
{
    public static class DevOpsUtilExtensions
    {
        public static BuildKey GetBuildKey(this Build build) => DevOpsUtil.GetBuildKey(build);

        public static BuildInfo GetBuildInfo(this Build build) => DevOpsUtil.GetBuildInfo(build);

        public static BuildDefinitionInfo GetBuildDefinitionInfo(this Build build) => DevOpsUtil.GetBuildDefinitionInfo(build);

        public static DateTimeOffset? GetStartTime(this Build build) => DevOpsUtil.ConvertRestTime(build.StartTime);

        public static DateTimeOffset? GetQueueTime(this Build build) => DevOpsUtil.ConvertRestTime(build.QueueTime);

        public static DateTimeOffset? GetFinishTime(this Build build) => DevOpsUtil.ConvertRestTime(build.FinishTime);

        public static int? GetByteSize(this BuildArtifact buildArtifact) => DevOpsUtil.GetArtifactByteSize(buildArtifact);

        public static BuildArtifactKind GetKind(this BuildArtifact buildArtifact) => DevOpsUtil.GetArtifactKind(buildArtifact);
    }
}