namespace Sonixy.FeedService.Helpers;

/// <summary>
/// HotScore Algorithm inspired by HackerNews/Reddit/EdgeRank.
/// Formula: HotScore = BaseTimeScore + log2(1 + WeightedInteractions) * BoostFactor
///
/// Key design decisions:
/// - BaseTimeScore = Unix timestamp ms (preserves chronological order for new posts)
/// - Interactions add a LOG-scaled bonus (diminishing returns: 1000→10 likes boost more than 1000→2000)
/// - Time decay is IMPLICIT: older posts have lower base scores, and new interactions give shrinking boosts
/// - The boost decays with post age, so old viral posts don't permanently dominate
/// </summary>
public static class HotScoreCalculator
{
    // Interaction weights
    private const double LikeWeight = 1.0;
    private const double CommentWeight = 3.0;
    private const double ShareWeight = 5.0;
    private const double ViewWeight = 0.1;
    private const double ReplyWeight = 2.0;

    // Boost factor: controls how much interactions can push a post up
    // Higher = interactions matter more vs. recency
    private const double BoostFactor = 3_600_000; // ~1 hour in ms worth of boost per log2 unit

    // Gravity: controls how fast the interaction boost decays with post age
    private const double GravityHoursHalfLife = 24.0; // After 24h, boost effectiveness halves

    /// <summary>
    /// Calculate the initial HotScore for a newly created post.
    /// Simply uses the creation timestamp so new posts appear at the top chronologically.
    /// </summary>
    public static double CalculateInitialScore(DateTime createdAt)
    {
        return new DateTimeOffset(createdAt).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Calculate the updated HotScore after an interaction event.
    /// </summary>
    /// <param name="createdAt">When the post was originally created</param>
    /// <param name="likeCount">Total likes on the post</param>
    /// <param name="commentCount">Total comments on the post</param>
    /// <param name="shareCount">Total shares (0 for now)</param>
    /// <param name="viewCount">Total views (0 for now)</param>
    public static double CalculateHotScore(
        DateTime createdAt,
        int likeCount = 0,
        int commentCount = 0,
        int shareCount = 0,
        int viewCount = 0)
    {
        var baseScore = CalculateInitialScore(createdAt);

        // Weighted interaction sum
        double weightedInteractions =
            (likeCount * LikeWeight) +
            (commentCount * CommentWeight) +
            (shareCount * ShareWeight) +
            (viewCount * ViewWeight);

        if (weightedInteractions <= 0)
            return baseScore;

        // Log-scaled boost (diminishing returns)
        double interactionBoost = Math.Log2(1 + weightedInteractions);

        // Time decay: the older the post, the less each interaction boosts it
        var postAgeHours = (DateTime.UtcNow - createdAt).TotalHours;
        double decayMultiplier = 1.0 / Math.Pow(1 + (postAgeHours / GravityHoursHalfLife), 1.5);

        // Final score = chronological base + decayed interaction boost
        return baseScore + (interactionBoost * BoostFactor * decayMultiplier);
    }
}
