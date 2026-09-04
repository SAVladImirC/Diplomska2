# DeepLM

Придружен код за дипломската работа **„Компаративна анализа на архитектурни шаблони кај .NET системи
и нивното влијание врз животниот век на софтверот“** (ФИНКИ, 2026).

Истиот мал домен (нарачки, клиенти, производи, правилото „10% попуст за нарачки над 100 денари“)
е имплементиран три пати, по еднаш за секоја архитектура од трудот, за да може директно да се
споредат по метриките од поглавјето „Споредба и евалуација“.

| Папка | Архитектура | Проекти |
|---|---|---|
| `src/01-NTier` | Повеќеслојна (N-tier) | `Presentation` → `Services` → `Data` |
| `src/02-CleanArchitecture` | Чиста (Clean) | `Domain` ← `Application` ← `Infrastructure`, `Presentation` |
| `src/03-VerticalSlice` | Вертикални исечоци (Vertical Slice) | `Web` (Features) → `Infrastructure` |
| `tests/*` | по еден тест-проект за секоја архитектура | |

## Извршување

```bash
dotnet build DeepLM.slnx
dotnet test DeepLM.slnx
dotnet run --project src/01-NTier/NTier.Presentation                          # http://localhost:5001
dotnet run --project src/02-CleanArchitecture/CleanArchitecture.Presentation  # http://localhost:5002
dotnet run --project src/03-VerticalSlice/VerticalSlice.Web                   # http://localhost:5003
```

Секоја апликација при старт ја мигрира својата SQLite база и ја пополнува со еден клиент и два
производа. Барањата за рачно тестирање се во `*.http` датотеката на секој startup проект.

## Каде во кодот е секој дел од трудот

### Теоретски основи

| Труд | Код |
|---|---|
| N-tier: презентациски, сервисен и податочен слој; DTO, валидација, ентитети, контекст, миграции | `NTier.Presentation/Controllers/OrdersController.cs`, `NTier.Services/{OrderService.cs,Dtos}`, `NTier.Data/{AppDbContext,Entities,Migrations}` |
| Clean: ентитети, вредносни објекти, доменски исклучоци, доменски настани | `CleanArchitecture.Domain/{Entities,ValueObjects,Exceptions,Events}` |
| Clean: апстракции, валидација, use cases, DTO мапирање | `CleanArchitecture.Application/{Abstractions,UseCases,Dtos}` |
| Clean: пристап до податоци, датотечен систем, мејл | `CleanArchitecture.Infrastructure/Persistence/*`, `Services/LocalFileStorage.cs`, `Services/ConsoleEmailService.cs` |
| Vertical Slice: `Features/Orders/<Slice>/` со endpoint, команда/прашање, обработувач, DTO | `VerticalSlice.Web/Features/Orders/{CreateOrder,GetOrders,UpdateOrder,DeleteOrder}` |
| Vertical Slice: инфраструктура без бизнис логика (перзистентност, надворешни сервиси, исклучоци) | `VerticalSlice.Infrastructure/{Persistence,Services,Common}` |
| CQRS | Vertical Slice: секој исечок е `IRequest` команда или прашање преку MediatR. Clean: `CreateOrder`/`UpdateOrder`/`SoftDeleteOrder` use cases пишуваат преку `IAddOrder`/`IUpdateOrder`/`IUnitOfWork`, а `GetOrders`/`GetOrderById` читаат преку посебен `IOrderQueries` → `OrderReadModel` (`EfOrderQueries`, `AsNoTracking` проекција). |

### SOLID

| Принцип | N-tier | Clean | Vertical Slice |
|---|---|---|---|
| SRP | `OrderService` ги содржи валидација, попуст, данок, мапирање и известување; правилото „барем една ставка“ живее и во DTO атрибутите (презентација) и во сервисот (`OrderServiceValidationTests`) | `ICreateOrderUseCase`, `ICalculateOrderTotalUseCase`, `IValidateOrderUseCase` … по еден метод | еден обработувач по исечок |
| OCP | нова функција = измена на `IOrderService`, `OrderService`, репозиториум и контролер | нов use case = нова класа + DI регистрација (`Program.cs`) | нов исечок = нова папка, ништо постоечко не се допира |
| LSP | `OrderRepository.Update/Delete` фрлаат за испорачани нарачки зад генеричкиот `IRepository<T>` (`OrderRepositoryLspTests`); `ExpressOrderService` ја прескокнува проверката на клиент на која базниот `OrderService` се потпира (`OrderServiceInheritanceLspTests`) | тесни интерфејси `IGetOrders`, `IAddOrder`, `ISoftDeleteOrder`; `PriorityOrder : Order` записи (`OrderTests`) | нема наследување меѓу исечоци; заедничкото е само `IEmailService` |
| ISP | контролерот и `FakeOrderService` мора да го носат целиот `IOrderService` (`OrdersControllerIspTests`) | контролерот зависи само од use case интерфејсите што ги повикува | обработувачите немаат сопствени интерфејси; `IEmailService` има еден метод |
| DIP | `Services` референцира `Data`; `IRepository<T>` живее во податочниот слој | апстракциите се во `Application`, EF/датотеки/мејл се во `Infrastructure`; `Application` нема референца кон EF | обработувачите користат `AppDbContext` директно (чиста форма) |

### Останати метрики

- **Флексибилност и ефект на бранување** – спореди што треба да се смени за ново правило:
  `OrderService` + `IOrderService` + контролер (N-tier), еден use case (Clean), една папка (Vertical Slice).
  Нова валидација кај N-tier значи измена на `CreateOrderRequest` атрибутите и на `OrderService`;
  кај Clean само `ValidateOrderUseCase`; кај Vertical Slice само валидаторот на исечокот.
- **Тестабилност** – `NTier.Tests` бара in-memory база и целосен fake на `IOrderService`;
  `CleanArchitecture.Tests` тестира домен без инфраструктура и use cases само со mock-ови на нивните
  директни зависности; `VerticalSlice.Tests` тестира секој обработувач со in-memory `AppDbContext`.
- **Архитектурна ерозија и технички долг** – N-tier: „дебел“ сервис и генерички репозиториум, а
  `OrdersController.GetOrder` е типичната „кратенка“: го прескокнува сервисот, чита директно од
  `AppDbContext` и враќа ентитет наместо DTO (`OrdersControllerIspTests.GetOrder_BypassesTheServiceLayerEntirely`);
  Clean: број на интерфејси и класи по use case; Vertical Slice: правилото за попуст и валидацијата
  намерно се дуплирани во `CreateOrder`, `UpdateOrder` и `GetOrders`
  (`UpdateOrderCommandHandlerTests.Handle_MatchesCreateOrder_ForTheSameDiscountRule` чува да не се
  разидат).
- **Доменски настани (Clean)** – `Order.Create` подига `OrderCreatedDomainEvent`; `EfUnitOfWork` го
  зачувува агрегатот, па преку `IDomainEventDispatcher` го предава на
  `OrderCreatedDomainEventHandler`, кој праќа потврда преку `ISendOrderConfirmationUseCase`.

### Сумарен преглед од трудот

| Метрика | N-tier | Clean | Vertical Slice |
|---|---|---|---|
| SOLID | слаба | многу силна | силна (без DIP) |
| CQRS | неприродна | природна | многу природна |
| Бранување | ниска флексибилност | локализирано на use case | максимална |
| Тестабилност | основна | одлична | одлична |
| Ерозија / долг | висок ризик | прекумерна апстракција | еволутивен (дуплирање) |
| Крива на учење | блага, потоа стрмна | стрмна, потоа стабилна | блага до умерена |
| Когнитивно оптоварување | високо низ слоеви | високо низ индирекции | ниско локално, високо глобално |
