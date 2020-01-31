# Redux Saga Error Handling

31 Jan 2020

```typescript
export function* requestCat() {
    try {
        const resp  = yield call(catService.get)
	
        yield put(actions.requestCatSuccess(resp))
    } catch (err) {
        yield put(actions.requestCatError(err))
    }
}

export function* catSaga() {
    yield takeLatest(actions.GET_CARD_PRODUCT_REQUEST, requestCat)
}
```

```typescript
import * as actions from '../actions/cat'
import { catService } from '../catServices'
import { put, call, takeLatest } from 'redux-saga/effects'

// In case we don't want error to bubble up to root Saga,
// we safe-wrapp it around individual saga generator.
const safe = (saga:any, ...args:any) => function* (action:any) {
  try {
    yield call(saga, ...args, action)
  } catch (err) {
    yield put(actions.httpError(err))
  }	
}

export function* requestCat() {
  const cats = yield call(catService.get)
  yield put(actions.requestCatSuccess(cats))
}

export function* requestCatSaga() {
    yield takeLatest(actions.GET_CAT_REQUEST, safe(requestCat))
}
```

```typescript
import { createStore, applyMiddleware, compose } from 'redux'
import RootReducer from './reducers'
import createSagaMiddleware from 'redux-saga'
import rootSaga from './sagas'

//option 1
const sagaMiddleware = createSagaMiddleware({
  onError: () => {
    store.dispatch({ type: 'HTTP_ERROR' })
  }
})

const composeEnhancers = ((window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ &&
  (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__({ trace: true, traceLimit: 25 })) 
  || compose

const store = createStore(
  RootReducer,
  composeEnhancers(applyMiddleware(sagaMiddleware))
)

// option 2
sagaMiddleware.run(rootSaga).done.catch((e:any) => {
  console.log({ message: e.message, source: 'sagaError', stacktrace: e.sagaStack })
  store.dispatch({ type: 'HTTP_ERROR' })
})

export default store
```
