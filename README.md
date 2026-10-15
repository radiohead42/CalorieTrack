# CalorieTrack

CalorieTrack es una aplicación para registrar alimentación, calorías y nutrientes, además de llevar seguimiento del perfil y objetivos personales de cada usuario.

El proyecto está pensado inicialmente como una aplicación sencilla y ligera, evitando arquitecturas innecesariamente complejas durante el MVP.

---

# Stack tecnológico

## Backend

- .NET 10
- ASP.NET Core REST API
- Controllers
- Entity Framework Core 10
- PostgreSQL
- Npgsql
- ASP.NET Core Identity
- JWT Bearer Authentication
- FluentValidation

## Base de datos

- PostgreSQL
- PostgreSQL ejecutándose mediante Docker
- Entity Framework Core Code First
- Migrations

## Frontend

Planificado:

- Angular 22
- Standalone Components
- Angular Material
- SCSS
- Signals

---

# Arquitectura actual

Por ahora se utiliza un único proyecto de API para mantener el proyecto ligero.

```text
CalorieTrack/
│
├── src/
│   └── CalorieTrack.Api/
│       │
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Models/
│       ├── Services/
│       ├── Validators/
│       ├── Migrations/
│       │
│       ├── Program.cs
│       ├── appsettings.json
│       └── CalorieTrack.Api.csproj
│
└── CalorieTrack.slnx
```

La idea es mantener responsabilidades separadas mediante carpetas y servicios sin introducir todavía múltiples proyectos o Clean Architecture.

---

# Base de datos

La aplicación utiliza PostgreSQL mediante Entity Framework Core.

Durante desarrollo PostgreSQL se ejecuta dentro de Docker y se expone por:

```text
localhost:5432
```

La conexión se almacena mediante .NET User Secrets para evitar guardar credenciales dentro de Git.

Ejemplo conceptual:

```text
ConnectionStrings:DefaultConnection
```

La contraseña real de PostgreSQL no debe almacenarse en `appsettings.json`.

---

# Entity Framework Core

El contexto principal es:

```text
ApplicationDbContext
```

Ubicado en:

```text
Data/ApplicationDbContext.cs
```

Actualmente administra:

```text
ApplicationUser
UserProfile
Food
ASP.NET Core Identity
```

Las migraciones permiten mantener sincronizado el modelo de EF Core con PostgreSQL.

Migraciones realizadas durante el desarrollo incluyen:

```text
InitialCreate
AddIdentity
ExpandUserProfile
AddFood
```

---

# Autenticación

CalorieTrack utiliza:

```text
ASP.NET Core Identity
+
JWT Bearer Authentication
```

El usuario está representado mediante:

```csharp
ApplicationUser : IdentityUser<Guid>
```

Por lo tanto, los usuarios utilizan `Guid` como identificador.

---

# Registro

Endpoint:

```http
POST /api/auth/register
```

Ejemplo:

```json
{
  "email": "kevin@example.com",
  "password": "Password123"
}
```

El flujo es:

```text
Request
   ↓
FluentValidation
   ↓
AuthController
   ↓
UserManager
   ↓
ASP.NET Core Identity
   ↓
PostgreSQL
```

La contraseña nunca se guarda directamente desde nuestra aplicación.

ASP.NET Core Identity se encarga de su administración.

---

# Login

Endpoint:

```http
POST /api/auth/login
```

Ejemplo:

```json
{
  "email": "kevin@example.com",
  "password": "Password123"
}
```

Si las credenciales son correctas se genera un JWT.

Ejemplo conceptual de respuesta:

```json
{
  "token": "eyJ...",
  "user": {
    "id": "GUID",
    "email": "kevin@example.com"
  }
}
```

---

# JWT

La configuración no secreta se encuentra en:

```json
{
  "Jwt": {
    "Issuer": "CalorieTrack.Api",
    "Audience": "CalorieTrack.Client",
    "ExpirationMinutes": 60
  }
}
```

La clave privada se almacena mediante User Secrets:

```text
Jwt:Key
```

El JWT contiene información mínima del usuario:

```text
sub
email
jti
```

No se almacenan datos como:

- contraseña
- peso
- altura
- información nutricional

Los endpoints privados utilizan:

```csharp
[Authorize]
```

Las peticiones autenticadas envían:

```http
Authorization: Bearer TOKEN
```

---

# Perfil del usuario

Cada usuario puede tener su propio perfil.

La relación conceptual es:

```text
ApplicationUser
       │
       │ 1:1
       ▼
  UserProfile
```

`UserProfile` contiene actualmente:

```text
Id
UserId
HeightCm
CurrentWeightKg
DateOfBirth
Sex
ActivityLevel
Goal
CreatedAtUtc
```

---

# Sexo

```text
Male
Female
```

---

# Nivel de actividad

Actualmente existen:

```text
Sedentary
LightlyActive
ModeratelyActive
VeryActive
ExtraActive
```

Estos valores posteriormente se utilizan para estimar el gasto calórico diario.

---

# Objetivo

Los objetivos disponibles actualmente son:

```text
LoseWeight
MaintainWeight
GainWeight
```

---

# API del perfil

## Obtener perfil

```http
GET /api/profile
```

Requiere JWT.

Ejemplo:

```bash
curl http://localhost:5063/api/profile \
  -H "Authorization: Bearer $TOKEN"
```

---

## Crear o actualizar perfil

```http
PUT /api/profile
```

Requiere JWT.

Ejemplo:

```json
{
  "heightCm": 175,
  "currentWeightKg": 75,
  "dateOfBirth": "1995-05-20",
  "sex": 1,
  "activityLevel": 3,
  "goal": 1
}
```

La API obtiene el usuario directamente desde el JWT.

El frontend no envía:

```text
UserId
```

Esto evita que un usuario pueda simplemente proporcionar el identificador de otro usuario para modificar su perfil.

El flujo es:

```text
JWT
 ↓
sub
 ↓
UserId
 ↓
UserProfile
 ↓
PostgreSQL
```

---

# Cálculo de calorías

Existe:

```text
CalorieCalculatorService
```

Su responsabilidad es estimar:

```text
BMR
Maintenance Calories
Target Calories
```

El cálculo utiliza:

```text
peso
altura
edad
sexo
actividad física
objetivo
```

## Metabolismo basal

Se utiliza la ecuación de Mifflin-St Jeor.

Para hombres:

```text
BMR =
10 × peso(kg)
+ 6.25 × altura(cm)
- 5 × edad
+ 5
```

Para mujeres:

```text
BMR =
10 × peso(kg)
+ 6.25 × altura(cm)
- 5 × edad
- 161
```

---

# Factores de actividad

```text
Sedentary          1.2
LightlyActive      1.375
ModeratelyActive   1.55
VeryActive         1.725
ExtraActive        1.9
```

Las calorías de mantenimiento se calculan mediante:

```text
Maintenance = BMR × ActivityFactor
```

---

# Objetivo calórico

Por ahora el MVP utiliza un ajuste sencillo de:

```text
500 kcal
```

Entonces:

```text
LoseWeight
Maintenance - 500

MaintainWeight
Maintenance

GainWeight
Maintenance + 500
```

Este valor es una decisión inicial de implementación y podrá hacerse configurable posteriormente.

---

# Alimentos

Se agregó el modelo:

```text
Food
```

Actualmente contiene:

```text
Id
Name
Brand
ExternalId

Calories
Protein
Carbohydrates
Fat
Fiber
Sugar
Sodium

CreatedAtUtc
```

Los nutrientes se normalizan por:

```text
100 g
```

Esto permitirá posteriormente hacer cálculos según la cantidad consumida.

Ejemplo:

```text
Food
165 kcal / 100 g

Cantidad consumida
150 g

Calorías
165 × 1.5
= 247.5 kcal
```

---

# Catálogo externo de alimentos

La integración elegida inicialmente es Open Food Facts.

La aplicación no expondrá directamente el modelo del proveedor.

La estructura es:

```text
Open Food Facts
       ↓
OpenFoodFactsService
       ↓
IFoodSearchService
       ↓
FoodSearchResult
       ↓
FoodsController
       ↓
Angular
```

Esto permite reemplazar el proveedor posteriormente sin cambiar los contratos principales del frontend.

---

# Búsqueda externa

Endpoint previsto/implementado:

```http
GET /api/foods/search?query=...
```

Por ejemplo:

```http
GET /api/foods/search?query=coca%20cola
```

El endpoint requiere autenticación.

Ejemplo:

```bash
curl \
  "http://localhost:5063/api/foods/search?query=coca%20cola" \
  -H "Authorization: Bearer $TOKEN"
```

---

# IFoodSearchService

La búsqueda externa se abstrae mediante:

```csharp
public interface IFoodSearchService
{
    Task<IReadOnlyCollection<FoodSearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}
```

Actualmente su implementación es:

```text
OpenFoodFactsService
```

Pero en el futuro podría cambiarse por otro proveedor:

```text
IFoodSearchService
        │
        ├── OpenFoodFactsService
        │
        └── OtroProveedor
```

---

# Open Food Facts

La respuesta externa contiene datos como:

```text
code
product_name
brands
nutriments
```

Dentro de `nutriments` encontramos valores como:

```text
energy-kcal_100g
proteins_100g
carbohydrates_100g
fat_100g
fiber_100g
sugars_100g
sodium_100g
```

Estos datos se transforman a nuestro propio DTO:

```text
FoodSearchResult
```

Ejemplo:

```json
{
  "externalId": "57045399",
  "name": "Coca-Cola",
  "brand": "coca cola",
  "calories": 42,
  "protein": 0,
  "carbohydrates": 10.6,
  "fat": 0,
  "fiber": 0,
  "sugar": 10.6,
  "sodium": 0
}
```

Por lo tanto, ni Angular ni el resto de nuestra API tienen que conocer la estructura interna de Open Food Facts.

---

# Puerto local

Durante desarrollo actualmente utilizamos:

```text
http://localhost:5063
```

Ejemplo:

```text
http://localhost:5063/api/auth/login
http://localhost:5063/api/profile
http://localhost:5063/api/foods/search
```

---

# Seguridad actual

Actualmente contamos con:

- ASP.NET Core Identity
- Contraseñas administradas mediante Identity
- JWT Bearer Authentication
- Endpoints protegidos con `[Authorize]`
- Clave JWT fuera del repositorio
- Connection String fuera del repositorio
- Datos privados relacionados mediante `UserId`
- El `UserId` se obtiene del JWT cuando corresponde
- DTOs separados de las entidades de EF Core
- Validación mediante FluentValidation

---

# Flujo general actual

```text
                  Angular
                 (futuro)
                     │
                     │ HTTP
                     ▼
              ASP.NET Core API
                     │
        ┌────────────┼────────────┐
        │            │            │
        ▼            ▼            ▼
  AuthController ProfileController FoodsController
        │            │            │
        ▼            ▼            ▼
    Identity      EF Core    IFoodSearchService
        │            │            │
        │            │            ▼
        │            │    OpenFoodFactsService
        │            │            │
        ▼            ▼            ▼
      PostgreSQL            Open Food Facts
```

---

# Estado actual

Actualmente funcionan o están preparados:

- [x] Proyecto .NET 10
- [x] REST API con Controllers
- [x] PostgreSQL
- [x] PostgreSQL mediante Docker
- [x] Entity Framework Core
- [x] Migrations
- [x] ASP.NET Core Identity
- [x] Registro de usuarios
- [x] Login
- [x] JWT
- [x] Endpoints autorizados
- [x] Perfil por usuario
- [x] Altura y peso
- [x] Fecha de nacimiento
- [x] Nivel de actividad
- [x] Objetivo
- [x] Cálculo de calorías objetivo
- [x] Modelo Food
- [x] Nutrientes por 100 g
- [x] Integración inicial con búsqueda externa
- [ ] Importar alimento externo a PostgreSQL
- [ ] Crear alimentos manualmente
- [ ] Meals
- [ ] MealItems
- [ ] Diario de alimentación
- [ ] Totales diarios
- [ ] Historial
- [ ] Seguimiento de peso
- [ ] Agua
- [ ] Ejercicio
- [ ] Funciones compartidas entre usuarios
- [ ] Frontend Angular
- [ ] Deploy

---

# Próximos pasos

El siguiente flujo será:

```text
Buscar alimento
        ↓
Open Food Facts
        ↓
Seleccionar alimento
        ↓
POST /api/foods/import
        ↓
Guardar copia en Foods
        ↓
PostgreSQL
```

Después construiremos:

```text
Meal
 │
 ├── Breakfast
 ├── Lunch
 ├── Dinner
 └── Snack
       │
       ▼
   MealItems
       │
       ├── Food
       └── QuantityGrams
```

Esto nos permitirá finalmente calcular:

```text
                  Objetivo
                 2100 kcal
                     │
                     ▼
               Día actual
                     │
        ┌────────────┼────────────┐
        ▼            ▼            ▼
    Desayuno       Comida        Cena
     450 kcal      700 kcal      550 kcal
        │            │            │
        └────────────┼────────────┘
                     ▼
                  Total
                1700 kcal
                     │
                     ▼
              Restantes: 400
```

---

# Objetivo del MVP

El primer MVP debería permitir:

1. Crear una cuenta.
2. Iniciar sesión.
3. Configurar perfil.
4. Calcular objetivo calórico.
5. Buscar alimentos.
6. Guardar alimentos.
7. Registrar alimentos consumidos.
8. Consultar calorías y nutrientes diarios.
9. Consultar historial.
10. Registrar evolución de peso.

Una vez terminado este flujo podremos empezar el frontend Angular sobre una API que ya tenga contratos y funcionalidades estables.
