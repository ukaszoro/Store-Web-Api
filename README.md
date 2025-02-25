## Web Store Api

Basic WebApi with in-memory database with the ability to add/remove products and for clients to negotiate prices. Authorized personal can reject/accept client bids.

## List of Endpoints

### `/api/Session/login`

- POST: Used for authorized workers to log in. Gives back a cookie that is later used in authorizing various restricted actions. No Data is needed for this action.
    
### `/api/Session/check`

- GET: Used for checking if user is logged in. Returns `OK` if logged in and `Unauthorized` if not logged in. (Debug purposes) No Data is needed for this action.

### `/api/Products`

- GET: Used to display all Products currently available in the database. No Data is needed for this action.

- POST: Used for adding new Products to the database. Requires the authorization cookie from `/api/Session/login`. Example Data for adding Products in json.

```json
{
  "Id": 1,
  "name": "Book",
  "price": 15
}
```

`Id` isn't used here, `name` of the product cannot be empty, `price` has to be > 0.


### `/api/Products/{Id}`

- GET: Used to request data about a specific Product. No Data is needed for this action.

- PUT: Used to change/edit Product information. Requires the authorization cookie from `/api/Session/login`. Example Data for editing Products in json.

```json
{
  "Id": 1,
  "name": "Book",
  "price": 20
}
```

Both the `Id` in json and `Id` from the endpoint need to be the same to work. Both `name` and `price` are overwritten.

- DELETE: Used to remove Products from the database completely. Removed the Product with the `Id` specified in the endpoint. Requires the authorization cookie from `/api/Session/login`.
    
### `/api/Products/{productId}/bids`

- GET: Used to display all negotiations for the Products with `productId` currently stored in the database. Requires the authorization cookie from `/api/Session/login`. No Data is needed for this action.

- POST: Used by customers to create/update a negotiation. Creates a cookie for distinguishing customers. Customer can have one negotiation per product and can edit his negotiation only if it's been rejected. Example Data for adding Negotiations in json.

```json
{
  "Id" : 1
  "productId": 1,
  "price": 15,
  "status": 1
}
```

`status` and `Id` is not used, `price` needs to be > 0, both the `productId` in json and `productId` from the endpoint need to be the same to work, the product also needs to exist.
            
### `/api/Products/{productId}/bids/{negotiationId}`

- GET: Used to display the negotiation with the `negotiationId` for the Product with `productId`. No Data is needed for this action.

- POST: Used to reject/accept negotiations. Requires the authorization cookie from `/api/Session/login`. Example Data for changing negotiation status in json.

```json
{
  "Id": 1,
  "productId": 1,
  "price": 1,
  "status": 2
}
```

`rejectedAt`, `timesRejected`, `price` are not used. id's in the json need to be the same as id's in the endpoint. `status` accepted values: `1` - accepting, `2` - rejecting.

- DELETE: Used to delete negotiation with the `negotiationId` for the Product with `productId`. Requires the authorization cookie from `/api/Session/login`. No Data is needed for this action.
