namespace SutomResolver.Tests;

[TestClass]
public class SutomConstraintsTests
{
    [TestMethod]
    public void FromGuessAndResult_ShouldCaptureMinAndMaxOccurrences_ForRepeatedLetters()
    {
        var result = SutomHelper.GetResultFromGuess("AABCC", "ABDDD");

        var constraints = SutomResolver.solver.SutomConstraints.FromGuessAndResult("AABCC", result);

        Assert.AreEqual('A', constraints.GetFixedLetter(0));
        Assert.IsTrue(constraints.IsLetterForbiddenAt(1, 'A'));
        Assert.IsTrue(constraints.IsLetterForbiddenAt(2, 'B'));
        Assert.IsTrue(constraints.IsLetterForbiddenAt(3, 'C'));
        Assert.IsTrue(constraints.IsLetterForbiddenAt(4, 'C'));

        Assert.IsTrue(constraints.TryGetLetterConstraint('A', out var aConstraint));
        Assert.AreEqual(1, aConstraint.Minimum);
        Assert.AreEqual(1, aConstraint.Maximum);

        Assert.IsTrue(constraints.TryGetLetterConstraint('B', out var bConstraint));
        Assert.AreEqual(1, bConstraint.Minimum);
        Assert.IsNull(bConstraint.Maximum);

        Assert.IsTrue(constraints.TryGetLetterConstraint('C', out var cConstraint));
        Assert.AreEqual(0, cConstraint.Minimum);
        Assert.AreEqual(0, cConstraint.Maximum);
    }

    [TestMethod]
    public void Matches_ShouldRejectCandidatesThatExceedTheMaximumRepeatedLetterCount()
    {
        var result = SutomHelper.GetResultFromGuess("AABCC", "ABDDD");
        var constraints = SutomResolver.solver.SutomConstraints.FromGuessAndResult("AABCC", result);

        Assert.IsTrue(constraints.Matches("ABDDD"));
        Assert.IsFalse(constraints.Matches("ABAAA"));
    }

    [TestMethod]
    public void FromGuessAndResult_ShouldCapLetterCount_WhenGuessContainsDuplicatesButTargetContainsOne()
    {
        var result = SutomHelper.GetResultFromGuess("AAFGH", "BACDE");

        var constraints = SutomResolver.solver.SutomConstraints.FromGuessAndResult("AAFGH", result);

        Assert.IsTrue(constraints.TryGetLetterConstraint('A', out var aConstraint));
        Assert.AreEqual(1, aConstraint.Minimum);
        Assert.AreEqual(1, aConstraint.Maximum);

        Assert.IsTrue(constraints.IsLetterForbiddenAt(0, 'A'));
        Assert.AreEqual('A', constraints.GetFixedLetter(1));

        Assert.IsTrue(constraints.Matches("BACDE"));
        Assert.IsFalse(constraints.Matches("BAADE"));
    }
}
