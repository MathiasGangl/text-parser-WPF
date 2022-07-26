using System.Collections.Generic;
using System.Linq;

namespace TextParsingLibrary
{
    public class TextStatistics
    {
        public Dictionary<string, int> WordFrequency { get; private set; } = new Dictionary<string, int>();
        public int WordCount { get; set; }

        /// <summary>
        /// Used to add a word with the given frequency to the dictionary.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="frequency"></param>
        public void AddWordFrequency(string word, int frequency)
        {
            try
            {
                WordFrequency[word] = frequency;
            }
            catch (KeyNotFoundException)
            {
                WordFrequency.Add(word, frequency);
            }

        }

        /// <summary>
        /// Used to increment the frequency of the given word in the dictionary.
        /// </summary>
        /// <param name="word"></param>
        public void IncrementWordFrequency(string word)
        {
            try
            {
                WordFrequency[word]++;
            }
            catch (KeyNotFoundException)
            {
                WordFrequency.Add(word, 1);
            }
        }

        /// <summary>
        /// Gets the frequency of the word specified.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>the word freuqency</returns>
        public int GetWordFrequency(string word)
        {
            try
            {
                return WordFrequency[word];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Clears the statistics.
        /// </summary>
        public void Clear()
        {
            WordCount = 0;
            WordFrequency.Clear();
        }

        /// <summary>
        /// Sorts the dictionary either ascending (true) or descenting (false).
        /// The default sorting is descending.
        /// </summary>
        /// <param name="ascending"></param>
        public void SortWordFrequency(bool ascending = false)
        {
            IOrderedEnumerable<KeyValuePair<string, int>> query;
            if (ascending)
            {
                query = from entry in WordFrequency orderby entry.Value ascending select entry;
            }
            else
            {
                query = from entry in WordFrequency orderby entry.Value descending select entry;
            }
            this.WordFrequency = query.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
