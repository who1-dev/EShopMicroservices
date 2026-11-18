# BuildingBlocks.Contracts

Plain values that more than one service exchanges in messages or API calls
(for example `PaymentMethod`, sent by Basket in `BasketCheckoutEvent` and stored by Ordering).
Domain projects may reference it, so it has to stay tiny.

## Rules

1. **No dependencies.** No NuGet packages and no project references. Anything added here is
   pulled into every domain that references it.
2. **Only values that cross a service boundary.** If only one service uses a type, it stays in
   that service (e.g. `OrderStatus` lives in `Ordering.Domain`).
3. **Never renumber or remove an enum value.** Messages already in RabbitMQ and rows already
   stored as `int` would silently change meaning. Add new values at the end.
4. **Receivers must reject unknown values.** A sender can be deployed with a new value before the
   receiver knows it: validate with `IsInEnum()`.
5. **Values only, no behavior.** Business rules belong in each service's domain. Shared logic here
   would couple the services together.

Warning sign: if a change here forces you to redeploy services that don't exchange that data,
it doesn't belong here.
