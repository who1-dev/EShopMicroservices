# EShopMicroservices
A C# .NET Core microservices implementation for an Electronic shop developed in a microservice pattern

## Running locally

```bash
cd src
docker compose up -d
```

Keycloak imports the `eshop` realm from `src/keycloak/eshop-realm.json` on first start.

### Demo credentials (local development only)

| Where | Username | Password | Role |
|---|---|---|---|
| Storefront / `eshop` realm | `customer` | `customer` | customer |
| Storefront / `eshop` realm | `storeadmin` | `storeadmin` | admin |
| Keycloak admin console (http://localhost:8080) | `admin` | `admin` | Keycloak admin |

These accounts exist only in the local Docker setup and must never be used in a deployed environment.
