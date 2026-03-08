# OrgaX - Security & Anti-DDoS Standard

## 8.2 Security Principles
- JWT/OAuth2 mandatory.
- NO hard-coded secrets.
- Rate limiting at both Gateway & Service layers.
- Secure-by-default architecture.

## 13.9 Security & Anti-DDoS Standard
- **Anti-DDoS (Rate Limiting)**:
    - Mandatory Rate Limiting implementation.
    - **Policy**: Default 100 requests / 1 minute / IP.
    - Return `429 Too Many Requests`.
- **Security Headers**:
    - **HSTS**: Force HTTPS.
    - **X-Frame-Options**: `DENY`.
    - **X-Content-Type-Options**: `nosniff`.
    - **CSP**: Content-Security-Policy mandatory.
- **CORS Policy**:
    - Only allow Whitelisted domains. Strictly NO `AllowAnyOrigin()` in production.
- **Data Validation**:
    - Mandatory input validation via FluentValidation.
    - HTML Sanitization for all user-provided text fields.
- **JWT Security**:
    - Mandatory validation of signature, expiration, Issuer, and Audience.
