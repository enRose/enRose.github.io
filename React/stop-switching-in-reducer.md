# How to reduce switch statement

31 Jan 2020

We've all done it - copy from tutorial, stick it in production code and never look back! So this is what we end up:

```typescript
switch (action.type) {
  case actions.GET_CAT_REQUEST:
    return { ...state, isFetching: true }

  case actions.GET_CAT_SUCCESS:
    return { ...state, isFetching: false, payload: action.payload }

  case actions.GET_CAT_ERROR:
    return { ...state, isFetching: false, error: action.error }

  // switch case will go on and on...

  default:
    return state
}
```

Okay, let's apply some good old Unix programming philosophy - Fold intelligence into data structure so your program can be simple and stupid.

Here is how:

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

We use a data literal called which. Its keys are action type, properties are a lambda function returns a new state.

By doing this, we fold the correlation (knowledge)between action type and its handler into a data structure so our program can be as stupid as ```which[action.type]```.

Thanks for reading if you have any suggestions please PR to my Github.