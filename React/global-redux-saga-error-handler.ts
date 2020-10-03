import { createStore, applyMiddleware, compose } from 'redux'
import RootReducer from './reducers'
import createSagaMiddleware from 'redux-saga'
import rootSaga from './sagas'
import { updateErrors } from './actions'
import { ErrorTypes } from '../models'

const sagaMiddleware = createSagaMiddleware({
	onError: (e: any) => {
		store.dispatch(updateErrors({
			type: ErrorTypes.TechnicalError,
			errorCode: e.status || e.responseCode || e.statusCode
		}))
	}
})

const composeEnhancers = ((window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ &&
	(window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__({ trace: true, traceLimit: 25 })) || compose

const store = createStore(
	RootReducer,
	composeEnhancers(applyMiddleware(sagaMiddleware))
)

sagaMiddleware.run(rootSaga)

export default store
