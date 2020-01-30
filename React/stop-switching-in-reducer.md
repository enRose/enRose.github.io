```typescript
switch (action.type) {
  case actions.GET_CARD_PRODUCT_REQUEST:
    return { ...state, isFetching: true }

  case actions.GET_CARD_PRODUCT_SUCCESS:
    return { ...state, isFetching: false, payload: action.payload }

  case actions.GET_CARD_PRODUCT_FAILURE:
    return { ...state, isFetching: false, error: action.error }

  default:
    return state
}
```

```typescript
import * as actions from '../actions/cats'

const firstCat: ICat = {
	isFetching: false,
	error: undefined,
	payload: undefined
}

export const cat = (state: ICat = firstCat, action: any) => {
	// Computed property names (ES2015)
	const which:any = {
		[actions.GET_CAT_REQUEST]: () => ({ ...state, isFetching: true }),
		[actions.GET_CAT_SUCCESS]: () => ({ ...state, isFetching: false, payload: action.payload }),
		[actions.GET_CAT_ERROR]: () => ({ ...state, isFetching: false, error: action.error }),
	}

	const reduce = which[action.type]

	return reduce ? reduce() : state
}
```
