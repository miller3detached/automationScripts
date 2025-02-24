namespace FrequencyAnalysis
{
    public partial class Program
    {
        public static class ArrayHelper
        {
            public static string[] GetUniqueWords(string[] words)
            {
                bool isWordUnique = true;
                int index = 0;
                string[] uniqueArray = new string[words.Length];
                for (int i = 0; i < words.Length; i++)
                {
                    for (int j = 0; j < words.Length; j++)
                    {
                        if (uniqueArray[j] == words[i])
                        {
                            isWordUnique = false;
                        }
                    }
                    if (isWordUnique) { uniqueArray[index] = words[i]; index++; } 
                }
                GetUniqueWordsWithoutEmptySpaces(ref uniqueArray);
                return uniqueArray;
            }
            public static void GetUniqueWordsWithoutEmptySpaces(ref string[] uniqueArray)
            {
                string[] result = uniqueArray.Where(i => i != null).ToArray();
                uniqueArray = result;
            }


            public static int[] GetWordsCount(string[] words, string[] uniqueWords)
            {
                int[] wordsCount = new int[uniqueWords.Length];
                int counter = 0;
                for (int i = 0; i < uniqueWords.Length; i++)
                {
                    counter = 0;
                    for (int j = 0; j < words.Length; j++)
                    {
                        if (uniqueWords[i] == words[j])
                        {
                            counter++;
                        }
                    }
                    wordsCount[i] = counter;
                }
                return wordsCount;
            }

            public static void SortArraysByCount(string[] uniqueWords, int[] wordsCount)
            {
                int maxIndex = 0;
                for (int i = 0; i < wordsCount.Length - 1; i++)
                {
                    maxIndex = i;
                    for (int j = i + 1; j < wordsCount.Length; j++)
                    {
                        if (wordsCount[maxIndex] < wordsCount[j])
                        {
                            maxIndex = j;
                        }
                    }
                    ReplaceNumbersAndWords(uniqueWords, wordsCount, maxIndex, i);
                }
            }
            public static void ReplaceNumbersAndWords(string[] uniqueWords, int[] wordsCount, int maxIndex, int index)
            {
                int tempA = wordsCount[index];
                wordsCount[index] = wordsCount[maxIndex];
                wordsCount[maxIndex] = tempA;

                ReplaceWords(uniqueWords, maxIndex, index);
            }

            public static void SortWordsWithEqualCountByAlphabet(string[] uniqueWords, int[] wordsCount)
            {
                int indexStartCounter = 0;
                int minWordIndex = 0;
                for (int i = indexStartCounter; i < uniqueWords.Length - 1; i++)
                {
                    minWordIndex = i;
                    for (int j = i + 1; j < uniqueWords.Length; j++)
                    {
                        if (wordsCount[i] == wordsCount[j])
                        {
                            indexStartCounter++;
                            bool isFirstWordLower = CompareWords(uniqueWords[minWordIndex], uniqueWords[j]);
                            if (!isFirstWordLower)
                            {
                                minWordIndex = j;
                            }
                        }
                    }
                    ReplaceWords(uniqueWords, minWordIndex, i);
                }
            }

            public static bool CompareWords(string firstWord, string secondWord)
            {
                bool isFirstWordLower = true;
                for (int i = 0; i < Math.Min(firstWord.Length, secondWord.Length); i++)
                {
                    if (firstWord[i] > secondWord[i])
                    {
                        return false;
                    }
                    if (firstWord[i] < secondWord[i])
                    {
                        return true;
                    }
                }
                if (firstWord.Length > secondWord.Length)
                {
                    return false;
                }
                return isFirstWordLower;
            }

            public static void ReplaceWords(string[] uniqueWords, int maxIndex, int index)
            {
                string tempB = uniqueWords[index];
                uniqueWords[index] = uniqueWords[maxIndex];
                uniqueWords[maxIndex] = tempB;
            }
                


        }
    }
}