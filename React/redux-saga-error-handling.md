# Redux Saga Error Handling

31 Jan 2020

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
	(window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__({ trace: true, traceLimit: 25 })) || compose

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
