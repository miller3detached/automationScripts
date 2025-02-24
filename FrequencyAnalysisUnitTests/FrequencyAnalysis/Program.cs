using System;
using System.Linq;

namespace FrequencyAnalysis
{
    public partial class Program
    {
        public static void Main()
         {
            string[] words = { "most", "of", "the", "adventures", "recorded", "in", "this",
            "book", "really", "occured;", "one", "or", "two", "were", "experiences", "of",
            "my", "own,", "the", "rest", "those", "of", "boys", "who", "were", "schoolmates",
            "of", "mine" };

            string[] uniqueWords = ArrayHelper.GetUniqueWords(words);
            int[] wordsCount = ArrayHelper.GetWordsCount(words, uniqueWords);
            ArrayHelper.SortArraysByCount(uniqueWords, wordsCount);
            ArrayHelper.SortWordsWithEqualCountByAlphabet(uniqueWords, wordsCount);

            for (int i = 0; i < uniqueWords.Length; i++)
            {
                Console.WriteLine(uniqueWords[i] + " - " + wordsCount[i]);
            }

            



        }
    }
}