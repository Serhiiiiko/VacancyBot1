using VacancyBot1.Services;

namespace VacancyBot1.Tests.Services;
public class CandidateServiceTests
{
    [Theory]
    [InlineData("+380501234567", true)]
    [InlineData("0501234567", false)]
    [InlineData("+38050123", false)]
    [InlineData("+3805012345678", false)]
    [InlineData("+380abcdefg", false)]
    public void IsValidPhoneNumber_ShouldValidateCorrectly(string phoneNumber, bool expected)
    {
        // Arrange
        var candidateService = new CandidateService(null, null, null, null, null);

        // Act
        var result = candidateService.IsValidPhoneNumber(phoneNumber);

        // Assert
        Assert.Equal(expected, result);
    }
}