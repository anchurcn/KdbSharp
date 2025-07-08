# PowerShell script to batch update test assertions in DeserializeTest.cs

$filePath = "Tests/KdbSharp.Test/DeserializeTest.cs"
$content = Get-Content $filePath -Raw

# Define test patterns and their expected values
$testPatterns = @{
    # Short type tests
    "Test_short_zero" = @{ type = "short"; expected = "(short)0" }
    "Test_short_null" = @{ type = "short"; expected = "short.MinValue" }
    "Test_short_inf" = @{ type = "short"; expected = "short.MaxValue" }
    "Test_short_ninf" = @{ type = "short"; expected = "-short.MaxValue" }
    "Test_short_max" = @{ type = "short"; expected = "(short)32766" }
    "Test_short_min" = @{ type = "short"; expected = "(short)-32766" }
    
    # Int type tests
    "Test_int_zero" = @{ type = "int"; expected = "0" }
    "Test_int_null" = @{ type = "int"; expected = "int.MinValue" }
    "Test_int_inf" = @{ type = "int"; expected = "int.MaxValue" }
    "Test_int_ninf" = @{ type = "int"; expected = "-int.MaxValue" }
    "Test_int_max" = @{ type = "int"; expected = "2147483646" }
    "Test_int_min" = @{ type = "int"; expected = "-2147483646" }
    
    # Long type tests
    "Test_long_zero" = @{ type = "long"; expected = "0L" }
    "Test_long_null" = @{ type = "long"; expected = "long.MinValue" }
    "Test_long_inf" = @{ type = "long"; expected = "long.MaxValue" }
    "Test_long_ninf" = @{ type = "long"; expected = "-long.MaxValue" }
    "Test_long_max" = @{ type = "long"; expected = "9223372036854775806L" }
    "Test_long_min" = @{ type = "long"; expected = "-9223372036854775806L" }
    
    # Float type tests
    "Test_real_zero" = @{ type = "float"; expected = "0f" }
    "Test_real_null" = @{ type = "float"; expected = "float.NaN" }
    "Test_real_inf" = @{ type = "float"; expected = "float.PositiveInfinity" }
    "Test_real_ninf" = @{ type = "float"; expected = "float.NegativeInfinity" }
    "Test_real_max" = @{ type = "float"; expected = "114514.1919810f" }
    "Test_real_min" = @{ type = "float"; expected = "-114514.1919810f" }
    
    # Double type tests
    "Test_float_zero" = @{ type = "double"; expected = "0.0" }
    "Test_float_null" = @{ type = "double"; expected = "double.NaN" }
    "Test_float_inf" = @{ type = "double"; expected = "double.PositiveInfinity" }
    "Test_float_ninf" = @{ type = "double"; expected = "double.NegativeInfinity" }
    "Test_float_max" = @{ type = "double"; expected = "114514.1919810" }
    "Test_float_min" = @{ type = "double"; expected = "-114514.1919810" }
}

# Function to replace TODO with actual assertion
function Replace-TestAssertion {
    param($testName, $type, $expected)
    
    $pattern = "public void $testName\(\)[\s\S]*?// TODO: Add specific deserialization and validation logic[\s\S]*?// Assert\.Equal\(expectedValue, result\);"
    
    $replacement = @"
public void $testName()
    {
        // Test deserialization of alltype.$($testName.Replace('Test_', ''))
        var data = _testDataReader.GetTestData("$($testName.Replace('Test_', ''))");
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var reader = new KSerializationReader(data) { IsLittleEndian = true };
        var result = KSerializer.Deserialize<$type>(ref reader);
        Assert.Equal($expected, result);
    }
"@
    
    return $content -replace $pattern, $replacement
}

# Apply replacements
foreach ($testName in $testPatterns.Keys) {
    $testInfo = $testPatterns[$testName]
    $content = Replace-TestAssertion $testName $testInfo.type $testInfo.expected
    Write-Host "Updated $testName"
}

# Write back to file
Set-Content $filePath $content -Encoding UTF8
Write-Host "Completed updating test assertions"
