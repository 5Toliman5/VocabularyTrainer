namespace VocabularyTrainer.Api.Contract.Words
{
    /// <summary>Specifies the direction of a training weight update.</summary>
    public enum WeightUpdateType
    {
        /// <summary>Increase the word's weight (user answered correctly).</summary>
        Increase,

        /// <summary>Decrease the word's weight (user answered incorrectly or skipped).</summary>
        Decrease
    }
}
