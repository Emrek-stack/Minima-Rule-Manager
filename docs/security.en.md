---
title: Security
layout: default
---

# Security

RuleEngine provides security and control points for enterprise usage.

## Security Policy

See `SECURITY.md` for the responsible disclosure process.

## Expression Safety

- Expressions are validated before compilation.
- Rules run only against defined input/output models.
- For custom evaluators, prefer isolation and whitelists.

## Audit and Traceability

- Rule execution audits capture who/when/result data.
- Define log retention based on your compliance requirements.

## Recommendations

- Keep DEBUG_RULES disabled in production.
- Use versioning and plan rollbacks for rule changes.
- Enable quota and usage checks for critical campaigns.
