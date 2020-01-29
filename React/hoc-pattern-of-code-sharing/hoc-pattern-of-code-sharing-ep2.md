# Advanced HOC
18 Jan 2020

Welcome back! Let's see what else HOC can help us with let's dig in.

# Example - input field decorator

## Motivation

Quite often we have fields that need state to be passed in and controlled value be dispatched/collected when changes. This is what React refers to as controlled component. We could write onChange to each input field individually and hook up whatever state management you have setup redux, flux, etc. However, this could quickly become an annoyance if we have tons of input components.

With HOC, we can wrap a component with redux state in one single HOC so anything we pass into this HOC will connect to redux state.

## 1st attempt

Let's don't worry about whether it compiles.

```typescript
const hoc = (id: string, fieldComponent: JSX.Element) => {
    const extendedProps = {
      id,
      name: id,
      type: fieldComponent.props.type,
      value: fields[id],
      onChange: (e: any) => onFieldChange(id, e.target.value),
    }

    return React.cloneElement(fieldComponent, extendedProps)
}

const Decorator = (id: string, fieldComponent: JSX.Element) => {
  return hoc(id, fieldComponent)
}
```

This looks familiar from [episode 1](https://enrose.github.io/React/hoc-pattern-of-code-sharing/hoc-pattern-of-code-sharing-ep1). This hoc takes an jsx element, adds some extra props to it using React.cloneElement(originalElement, extraProps).

When we run ```Decorator('xman', <input />)```, we then get a new input with extra bit of props to it.

Now, you understand how we can use React.cloneElement() to extend any component's props, let's change it a bit so we can run our Decorator like this: ```Decorator('xman')(<input />)```.

## 2nd draft

```typescript
const Decorator = (id: string) => {
  return (fieldComponent: JSX.Element) => {
    const extendedProps = {
      id,
      name: id,
      type: fieldComponent.props.type,
      value: fields[id],
      onChange: (e: any) => onFieldChange(id, e.target.value),
    }

    return React.cloneElement(fieldComponent, extendedProps)
  }
}
```

Now we can do:

```typescript
const hoc = Decorator('xman')

const enhancedInput = hoc(<input />)
```

This is effectively same as: ```Decorator('xman')(<input />)```

## connect to redux

Let's what we normally do when we connect a functional component to redux.

```typescript
const F = ({ state, onFieldChange }: any) => {
  // do something with state
  return null
}

connect(
  (state: any) => ({
    state: state,
  }),
  { onFieldChange },
)(F)
```

This F component although is connected to redux but it is not doing anything. Let's use the state and onFieldChange dispatch.

Now, this F function takes a fieldComponent, as well as state and a dispatch from redux and it adds extra props to this  fieldComponent and return a clone of it.

```typescript
const F = ({ fields, onFieldChange, fieldComponent }: any) => {
  const extendedProps = {
    id,
    name: id,
    type: fieldComponent.props.type,
    value: fields[id],
    onChange: (e: any) => onFieldChange(id, e.target.value),
  }

  return React.cloneElement(fieldComponent, extendedProps)
}

connect(
  (state: any) => ({
    fields: state.fields,
  }),
  { onFieldChange },
)(F)
```

Now if we assign the returned component from connect to a const then invoke React.createElement(el), we will get a react element ready to use without jsx. This is needed because our decorator is a function in TS file NOT TSX.

```typescript
const el = connect(
  (state: any) => ({
    fields: state.fields,
  }),
  { onFieldChange },
)(F)

const result = React.createElement(el)
```

## All in one

Here is all the bits we above put together, the only difference is we nest and return functions inside the body of Decorator to utilise JS closure.

```typescript
const Decorator = (id: string) => {
  return (fieldComponent: JSX.Element) => {

    const F = ({ fields, onFieldChange }: any) => {

      const extendedProps = {
        id,
        name: id,
        type: fieldComponent.props.type,
        value: fields[id],
        onChange: (e: any) => onFieldChange(id, e.target.value),
      }

      return React.cloneElement(fieldComponent, extendedProps)
    }

    const el = React.memo(connect(
      (state: any) => ({
        fields: state.fields,
      }),
      { onFieldChange },
    )(F))

    return React.createElement(el)
  }
}
```

There we have it. I use this pattern in one of my field-decorator package which is a bit more complex because it has to support quite a few features. But the gist of it is essentially the same. If you are interested in it here is the [article](https://enrose.github.io/React/decorator-pattern).

Source is available on [Github](https://github.com/enRose/react-field-decorator/tree/master/src)