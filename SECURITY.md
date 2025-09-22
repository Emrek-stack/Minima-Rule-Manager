# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take security bugs seriously. We appreciate your efforts to responsibly disclose your findings, and will make every effort to acknowledge your contributions.

### How to Report a Security Vulnerability

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please report them via email to: [security@yourdomain.com](mailto:security@yourdomain.com)

You should receive a response within 48 hours. If for some reason you do not, please follow up via email to ensure we received your original message.

Please include the following information in your report:

- Type of issue (e.g. buffer overflow, SQL injection, cross-site scripting, etc.)
- Full paths of source file(s) related to the manifestation of the issue
- The location of the affected source code (tag/branch/commit or direct URL)
- Any special configuration required to reproduce the issue
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the issue, including how an attacker might exploit it

### What to Expect

After you submit a report, we will:

1. Confirm receipt of your vulnerability report within 48 hours
2. Provide regular updates on our progress
3. Credit you in our security advisories (unless you prefer to remain anonymous)

### Security Best Practices

When using RuleEngine, please follow these security best practices:

#### Rule Compilation Security

- **Validate Input**: Always validate and sanitize rule input data
- **Sandboxing**: Consider running rule compilation in a sandboxed environment for untrusted rules
- **Resource Limits**: Set appropriate timeouts and memory limits for rule execution

```csharp
// Example: Safe rule compilation with timeout
var compiler = new RuleCompiler<MyInput, bool>();
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var rule = await compiler.CompileAsync("safe-rule", ruleString, cts.Token);
```

#### Database Security

- **Connection Strings**: Never hardcode database connection strings
- **SQL Injection**: Use parameterized queries (EF Core handles this automatically)
- **Access Control**: Implement proper authentication and authorization

```csharp
// Example: Secure connection string configuration
builder.Services.AddRuleEngine()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
```

#### API Security

- **Authentication**: Implement proper authentication for rule management APIs
- **Authorization**: Use role-based access control for rule operations
- **Rate Limiting**: Implement rate limiting to prevent abuse

```csharp
// Example: Secure API endpoint
[Authorize(Roles = "RuleManager")]
[HttpPost]
public async Task<IActionResult> CreateRule([FromBody] CreateRuleRequest request)
{
    // Implementation
}
```

### Known Security Considerations

#### Rule Compilation

- **Code Injection**: Rule strings are compiled as C# code. Only allow trusted users to create rules
- **Resource Exhaustion**: Malicious rules could consume excessive CPU or memory
- **Assembly Loading**: Compiled rules are loaded into the application domain

#### Recommendations

1. **Input Validation**: Always validate rule input data
2. **User Permissions**: Restrict rule creation to authorized users only
3. **Monitoring**: Monitor rule execution for unusual patterns
4. **Regular Updates**: Keep dependencies updated for security patches

### Security Updates

We will release security updates as needed. When a security vulnerability is discovered:

1. We will create a security advisory
2. We will release a patched version as soon as possible
3. We will notify users through GitHub releases and security advisories

### Contact

For security-related questions or concerns, please contact us at:
- Email: [security@yourdomain.com](mailto:security@yourdomain.com)
- GitHub Security Advisories: [View advisories](https://github.com/yourusername/RuleEngine/security/advisories)

### Acknowledgments

We would like to thank the following security researchers who have responsibly disclosed vulnerabilities:

- [List security researchers here]

---

**Last updated**: September 2025
