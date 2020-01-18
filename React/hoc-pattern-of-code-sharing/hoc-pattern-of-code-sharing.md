# What is HOC? I mean really

React calls it high order component. I personally like to think it as a JS function that takes a component or even multiple components as some of its parameters, and then adds extra bits to them.

It is simply a way to avoid writing repetitive code.

# Example - routing

We have a pagination situation that we need to navigation from by button click.

Without HOC, we might take the onClick handler out somewhere to share, we then would add onClick to all buttons.

```typescript
import React from "react"
import { useHistory, } from 'react-router-dom'

const next = (nextPageName:string, history:any) => {
    history.push(`/${nextPageName}`)
}

const About = () => {
 const history = useHistory()
    return <>
    <h2>About</h2>
        <button onClick={() => next('topics', history)}>next</button>
    </>
}
```

Here is HOC approach:

```typescript
import React from "react"
import { WithNav } from '../components/nav'

const About = () => {
    return (
        <>
        <h2>About</h2>
        {WithNav('about', <button>next</button>)}
        </>
    )
}

export default About
```

The button is now passed in as a parameter to a function - WithNav().

This the complete code of our HOC - nav.

```typescript
import { useHistory, } from 'react-router-dom'
import {pageRoutes} from '../pages/pageRoutes'
import React from 'react'

const next = (currentPageName:string, history:any) => {
    const nextPageIndex = pageRoutes.findIndex(
        r => r.name === currentPageName) + 1
    const nextPageName = (pageRoutes[nextPageIndex] || {}).name
    history.push(`/${nextPageName}`)
}

export const WithNav = (currentPageName:string, el:JSX.Element) => {
    const history = useHistory()
    return React.cloneElement(el, {onClick: () => next(currentPageName, history)})
}
```

**Let's go through a few things in detail:**
> useHistory() it won't work if we put it inside the onClick handler. It has to be called inside the body of a functional component.

```typescript
const next = (currentPageName:string) => {
    const nextPageIndex = pageRoutes.findIndex(
        r => r.name === currentPageName) + 1
    const nextPageName = (pageRoutes[nextPageIndex] || {}).name

    // this wont' work --^-------^--
    useHistory().push(`/${nextPageName}`)
}
```

> React.cloneElement() - it adds extra props onto an jsx element. It provide a way of adding cross-cutting code to any element controlled or uncontrolled.

```typescript
React.cloneElement(el, {onClick: () => next(currentPageName, history)})
}
```

> pageRoutes - a page flow configuration.

```typescript
export enum PageNames {
    home = 'home',
    about = 'about',
    topics = 'topics',
}
```

Provided the current page name, we work out the next page name then push it into history. The reason is we don't want each individual page to have to know what next page that it has to navigate to. It should only need to know its own name, let other module to have the knowledge of page flow.

This is a concept from Unix programming that we separate policy/configuration from operation. We will have another article explains it in detail.

```typescript
const nextPageIndex = pageRoutes.findIndex(
        r => r.name === currentPageName) + 1
const nextPageName = (pageRoutes[nextPageIndex] || {}).name
```
