using static FrequencyAnalysis.Program;

namespace FrequencyAnalysisUnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(new[] { "yes", "yes", "yes" }, new[] { "yes" })]
        [TestCase(new[] { "yes", "no", "yes" }, new[] { "yes", "no" })]
        [TestCase(new[] { "yes", "no", "yes", "no" }, new[] { "yes", "no" })]
        [TestCase(new[] { "yes" }, new[] { "yes" })]
        public void GetArrayWithUniqueWords_ShouldReturnUniqueArrayWithoutEmptySpaces(string[] words, string[] expectedResult)
        {
            string[] actualResult = ArrayHelper.GetUniqueWords(words);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(new[] { "yes", "yes", "yes" }, new[] { "yes" }, new[] { 3 })]
        [TestCase(new[] { "yes", "no", "yes" }, new[] { "yes", "no" }, new[] { 2, 1 })]
        [TestCase(new[] { "yes", "no", "yes", "no" }, new[] { "yes", "no" }, new[] { 2, 2 })]
        [TestCase(new[] { "yes" }, new[] { "yes" }, new[] { 1 })]
        public void GetArrayWithNumbers_ShouldReturnWordsCount(string[] words, string[] uniqueWords, int[] expectedResult)
        {
            int[] actualResult = ArrayHelper.GetWordsCount(words, uniqueWords);
            Assert.AreEqual(expectedResult, actualResult);
        }

        /*public void SortArraysByCount(string[] uniqueWords, int[] wordsCount)
        {
            SortArraysByCount(uniqueWords, wordsCount);
            int[] actualResult = 
            Assert.AreEqual(wordsCount, actualResult);
        }*/

        [TestCase("AAA", "AAAA", true)]
        [TestCase("AAA", "ABA", true)]
        [TestCase("BAA", "AAAA", false)]
        [TestCase("AAA", "AAA", true)]
        [TestCase("AAAA", "AAA", false)]
        [TestCase("AA", "B", true)]
        public void GetLowerWord_ReturnTrueForTheLowerWord(string firstWord, string secondWord, bool expectedResult)
        {
            bool actualResult = ArrayHelper.CompareWords(firstWord, secondWord);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(new[] { "B", "C", "D", "A" }, new[] { 1, 1, 1, 1 }, new[] { "A", "B", "C", "D" })]
        [TestCase(new[] { "B", "C", "AA", "D", "A" }, new[] { 1, 1, 1, 1, 1 }, new[] { "A", "AA", "B", "C", "D" })]
        public void GetSortedWords_ReturnArraySortedByAlphabet(string[] uniqueWords,int[] wordsCount, string[] expectedResult)
        {
            ArrayHelper.SortWordsWithEqualCountByAlphabet(uniqueWords, wordsCount);
            Assert.AreEqual(expectedResult, uniqueWords);
        }
    }
}