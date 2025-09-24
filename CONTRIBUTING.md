# Contributing to RuleEngine

Thank you for your interest in contributing to RuleEngine! This document provides guidelines and information for contributors.

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or JetBrains Rider
- Git

### Setting Up the Development Environment

1. **Fork and Clone**
   ```bash
   git clone https://github.com/your-username/RuleEngine.git
   cd RuleEngine
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run Tests**
   ```bash
   dotnet test
   ```

## 🏗️ Project Structure

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/          # Core rule engine functionality
│   ├── RuleEngine.Sqlite/        # SQLite persistence layer
│   └── RuleEngine.Mvc/           # Sample MVC application
├── tests/
│   ├── RuleEngine.Core.Tests/    # Unit tests for core functionality
│   └── RuleEngine.Integration.Tests/ # Integration tests
├── docs/                         # Documentation
└── samples/                      # Example applications
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/RuleEngine.Core.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Writing Tests

- Use **xUnit** for unit tests
- Use **FluentAssertions** for assertions
- Follow the **AAA pattern** (Arrange, Act, Assert)
- Test both success and failure scenarios

Example:
```csharp
[Fact]
public async Task RuleCompiler_ShouldCompileValidRule()
{
    // Arrange
    var compiler = new RuleCompiler<TestInput, bool>();
    var ruleString = "Input.Value > 10";

    // Act
    var result = compiler.CheckSyntax(ruleString);

    // Assert
    result.Should().BeEmpty();
}
```

## 📝 Code Style

### C# Coding Standards

- Follow **Microsoft C# Coding Conventions**
- Use **PascalCase** for public members
- Use **camelCase** for private fields
- Use **async/await** for asynchronous operations
- Use **nullable reference types** where appropriate

### Naming Conventions

- **Classes**: `PascalCase` (e.g., `RuleCompiler`)
- **Methods**: `PascalCase` (e.g., `CompileAsync`)
- **Properties**: `PascalCase` (e.g., `IsActive`)
- **Fields**: `camelCase` with underscore prefix for private (e.g., `_ruleRepository`)
- **Constants**: `PascalCase` (e.g., `DefaultTimeout`)

### Documentation

- Use **XML documentation** for public APIs
- Include **examples** in documentation
- Document **parameters** and **return values**
- Use **meaningful commit messages**

Example:
```csharp
/// <summary>
/// Compiles a C# rule string into an executable function.
/// </summary>
/// <typeparam name="TInput">The input type for the rule</typeparam>
/// <typeparam name="TReturn">The return type of the rule</typeparam>
/// <param name="ruleName">The name of the rule</param>
/// <param name="ruleString">The C# code to compile</param>
/// <returns>A compiled rule that can be executed</returns>
/// <example>
/// <code>
/// var compiler = new RuleCompiler&lt;CustomerInput, bool&gt;();
/// var rule = await compiler.CompileAsync("age-check", "Input.Age > 18");
/// var result = rule.Invoke(new CustomerInput { Age = 20 });
/// </code>
/// </example>
public async Task<CompiledRule<TInput, TReturn>> CompileAsync(string ruleName, string ruleString)
{
    // Implementation
}
```

## 🐛 Bug Reports

### Before Submitting

1. **Search existing issues** to avoid duplicates
2. **Test with the latest version** of RuleEngine
3. **Check if it's already fixed** in the main branch

### Bug Report Template

```markdown
**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Go to '...'
2. Click on '....'
3. Scroll down to '....'
4. See error

**Expected behavior**
A clear and concise description of what you expected to happen.

**Screenshots**
If applicable, add screenshots to help explain your problem.

**Environment:**
- OS: [e.g. Windows 10, macOS 12.0, Ubuntu 20.04]
- .NET Version: [e.g. 8.0.0]
- RuleEngine Version: [e.g. 1.0.0]

**Additional context**
Add any other context about the problem here.
```

## ✨ Feature Requests

### Before Submitting

1. **Check existing feature requests**
2. **Consider if it fits the project scope**
3. **Provide a clear use case**

### Feature Request Template

```markdown
**Is your feature request related to a problem? Please describe.**
A clear and concise description of what the problem is.

**Describe the solution you'd like**
A clear and concise description of what you want to happen.

**Describe alternatives you've considered**
A clear and concise description of any alternative solutions or features you've considered.

**Additional context**
Add any other context or screenshots about the feature request here.
```

## 🔄 Pull Request Process

### Before Submitting

1. **Create a feature branch** from `main`
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Make your changes** following the coding standards

3. **Add tests** for new functionality

4. **Update documentation** if needed

5. **Run all tests** to ensure nothing is broken
   ```bash
   dotnet test
   ```

6. **Commit your changes** with a clear message
   ```bash
   git commit -m "Add amazing feature"
   ```

7. **Push to your fork**
   ```bash
   git push origin feature/amazing-feature
   ```

### Pull Request Template

```markdown
**Description**
Brief description of the changes.

**Type of Change**
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

**Testing**
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

**Checklist**
- [ ] Code follows the project's coding standards
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests added/updated
```

## 🏷️ Release Process

### Versioning

We follow [Semantic Versioning](https://semver.org/):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Release Checklist

- [ ] All tests pass
- [ ] Documentation updated
- [ ] Version numbers updated
- [ ] CHANGELOG.md updated
- [ ] NuGet packages built
- [ ] Release notes prepared

## 📚 Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Roslyn Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [xUnit Testing](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)

## 🤝 Community Guidelines

### Code of Conduct

- **Be respectful** and inclusive
- **Be constructive** in feedback
- **Be patient** with newcomers
- **Be collaborative** in discussions

### Getting Help

- **GitHub Discussions** for questions and ideas
- **GitHub Issues** for bugs and feature requests
- **Pull Requests** for code contributions

## 📞 Contact

- **Maintainer**: [Your Name](mailto:your.email@example.com)
- **GitHub**: [@yourusername](https://github.com/yourusername)
- **Twitter**: [@yourusername](https://twitter.com/yourusername)

---

Thank you for contributing to RuleEngine! 🚀

