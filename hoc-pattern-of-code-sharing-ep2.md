# HOC a more involved scenario

Welcome back! Let's see what else HOC can help us with let's dig in.

# Example - input field decorator

## Motivation

Quite often we have fields that need state to be passed into and controlled value be dispatched/collected when change. This is what React refers to as controlled component. We could write onChange to each input field individually and hook up whatever state management you have setup redux, flux, etc. This could quickly become an annoyance if we have tons of input components.

Also when these fields have complex validation logics such as field-x is invalid when field-y contains 'thanos', or field-z has multiple scenarios to be validated each has a different error message. An example would be embossing name on credit card needs to validate: _A. if it is empty. B. if contains profane words. C. if it is within a certain length or it won't fit in on the card._

With HOC, we can address the issues above in a clear and declarative manner.

## Decorator

The Decorator is a function takes two parameters: id of the wrapped component and configuration object.

Configuration object has properties of: rules, initialValue, correlationId and show - visibility toggle.

```typescript
Decorator('Earth', {
	correlationId: 'marvel',
	rules: [
		{
			type: 'text',
		},
		{
			required: true,
			message: 'Please enter a Marvel name!',
		},
		{
			name: 'EarthRule',
			validator: IsThanosOnEarth,
			message: 'Thanos is on Earth',
		},
		{
			name: 'AsgardRule',
			validator: IsThanosInAsgard,
			message: 'Thanos is in Asgard',
		},
	],
	initialValue: 'This is Earth'
})
```

And it returns an HOC function that takes a React element as parameter. This HOC function in turn returns the wrapped component with extended props: id, name, type, value, onChange.

```typescript
(fieldComponent: JSX.Element) => {
  const F = ({ fields, onFieldChange }: any) => {
    let failedRules
    const onChange = (v: any) => {
      failedRules = RuleEngine(config.rules, v, fields)
      onFieldChange(id, v, failedRules, config.correlationId)
    }

    const extendedProps = {
      id,
      name: id,
      type: config.rules && (config.rules.find((r: any) => r.type) || {}).type || 'text',
      value: fields[id] === undefined ? config.initialValue : fields[id],
      onChange: (e: any) => onChange(e.target.value),
    }

    return React.cloneElement(fieldComponent, extendedProps)
  }

  const el = React.memo(connect(
    (state: any) => ({
      fields: state.fields,
    }),
    { onFieldChange },
  )(F))

  return config.show === false ? null : React.createElement(el)
}
```