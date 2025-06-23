🛠️ 1. Assign Roles via realm-management Client
Go to Keycloak Admin Console:

Users > testuser > Role Mappings

Under Client Roles, select realm-management

Assign the following roles:

view-users

query-users

manage-users

This is different from realm-level roles. You must assign them as client roles from realm-management:

✅ These roles are needed for API access:

realm-management:view-users

realm-management:manage-users