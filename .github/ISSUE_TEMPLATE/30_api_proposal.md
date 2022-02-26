---
name: API proposal
about: Propose a change to the public API surface
title: ''
labels: api-suggestion
assignees: ''

---

## Background and Motivation

<!--
We welcome API proposals! This template will help us gather the information we need to start the review process.
First, please describe the purpose and value of the new API here.
-->

## Proposed API

<!--
Please provide the specific public API signature diff that you are proposing. For example:
```diff
namespace scrapy.net
{
    public static class HttpResponseWritingExtensions
    {
+       public Task WriteAsync(this HttpResponse response, StringBuilder builder);
    }
}
```

-->

## Usage Examples

<!--
Please provide code examples that highlight how the proposed API additions are meant to be consumed.
This will help suggest whether the API has the right shape to be functional, performant and useable.
You can use code blocks like this:
``` C#
// some lines of code here
```
-->

## Alternative Designs

<!--
Were there other options you considered, such as alternative API shapes?
How does this compare to analogous APIs in other ecosystems and libraries?
-->

## Risks

<!--
Please mention any risks that to your knowledge the API proposal might entail, such as breaking changes, performance regressions, etc.
-->

