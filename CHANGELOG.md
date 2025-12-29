# Changelog

All notable changes to RuleEngine will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial release of RuleEngine
- Core rule compilation and execution engine
- SQLite persistence layer
- ASP.NET Core MVC sample application
- KnockoutJS rule builder UI
- Comprehensive test suite

## [1.0.0] - 2025-09-22

### Added
- **Core Rule Engine**
  - `RuleCompiler<TInput, TReturn>` for dynamic C# rule compilation
  - `RuleSet<TInput, TOutput>` for complex rule execution
  - `RuleManager` static class for rule management
  - Syntax validation with detailed error reporting
  - Async/await support for all operations
  - Type-safe generic input/output models

- **SQLite Persistence**
  - `IRuleRepository` for rule CRUD operations
  - `IAuditRepository` for execution auditing
  - Entity Framework Core integration
  - Database migrations and seeding
  - Rule versioning and rollback capabilities
  - Import/export functionality

- **MVC Sample Application**
  - Complete ASP.NET Core MVC application
  - Rule management UI with CRUD operations
  - Visual rule builder with KnockoutJS
  - Drag-and-drop rule construction
  - ACE Editor integration for C# code editing
  - Execution history and monitoring
  - Bootstrap-based responsive design

- **Rule Builder UI**
  - KnockoutJS-based rule builder
  - Drag-and-drop rule construction
  - Dynamic parameter management
  - Real-time validation
  - JSON-based rule serialization
  - Select2 dropdowns and Sortable.js integration

- **Testing**
  - Comprehensive unit test suite
  - Integration tests with SQLite
  - xUnit and FluentAssertions
  - Test coverage for core functionality
  - Mock-based testing for repositories

- **Documentation**
  - Complete README with examples
  - API documentation with XML comments
  - Contributing guidelines
  - Code of conduct
  - License (MIT)

### Technical Details
- **Target Framework**: .NET 8.0
- **Dependencies**: 
  - Microsoft.CodeAnalysis.CSharp.Scripting 4.8.0
  - Microsoft.EntityFrameworkCore.Sqlite 8.0.0
  - Microsoft.Extensions.DependencyInjection 8.0.0
- **Performance**: 
  - Rule compilation: ~50-100ms
  - Rule execution: ~0.1-1ms
  - Thread-safe concurrent execution
- **Features**:
  - Roslyn-based C# compilation
  - SQLite database with EF Core
  - RESTful API endpoints
  - Real-time rule validation
  - Comprehensive error handling

### Examples
- E-commerce pricing rules
- Loan approval workflows
- Customer validation logic
- Dynamic business rules
- Conditional logic execution

### NuGet Packages
- `RuleEngine.Core` - Core rule engine functionality
- `RuleEngine.Sqlite` - SQLite persistence layer

---

## Version History

### 1.0.0 (2025-09-22)
- Initial release
- Complete rule engine implementation
- SQLite persistence
- MVC sample application
- Comprehensive documentation

---

## Migration Guide

### Migration Notes

RuleEngine is a complete port of a legacy rule engine with modern .NET 8 features:

1. **Static RuleManager**: The core `RuleManager` remains static for compatibility
2. **RuleCompiler**: Enhanced with better error handling and async support
3. **RuleSet**: Improved with better type safety and performance
4. **Persistence**: Added SQLite support with EF Core
5. **UI**: Ported KnockoutJS frontend with modern styling

### Breaking Changes
- None in 1.0.0 - this is a new project

### New Features
- SQLite persistence layer
- ASP.NET Core MVC integration
- Enhanced error handling
- Better performance
- Modern .NET 8 features

---

## Support

- **GitHub Issues**: [Report bugs and request features](https://github.com/yourusername/RuleEngine/issues)
- **GitHub Discussions**: [Ask questions and share ideas](https://github.com/yourusername/RuleEngine/discussions)
- **Documentation**: [Complete documentation](https://github.com/yourusername/RuleEngine/wiki)

---

**Made with ❤️ for the .NET community**



