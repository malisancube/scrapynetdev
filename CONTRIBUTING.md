# How to contribute

One of the easiest ways to contribute is to participate in discussions on GitHub issues. You can also contribute by submitting pull requests with code changes.

## General feedback and discussions?

Start a discussion on the [repository issue tracker](https://github.com/malisancube/scrapynetdev/issues).

## Bugs and feature requests?

Before reporting a new issue, try to find an existing issue if one already exists. If it already exists, upvote (👍) it. Also, consider adding a comment with your unique scenarios and requirements related to that issue.  Upvotes and clear details on the issue's impact help us prioritize the most important issues to be worked on sooner rather than later. If you can't find one, that's okay, we'd rather get a duplicate report than none.

If you can't find an existing issue, log a new issue in the appropriate GitHub repository. Here are some of the most common repositories:

## How to submit a PR

We are always happy to see PRs from community members both for bug fixes as well as new features.
To help you be successful we've put together a few simple rules to follow when you prepare to contribute to our codebase:

### Finding an issue to work on

  Over the years we've seen many PRs targeting areas of the framework, which we didn't plan to expand further at the time.
  In all these cases we had to say `no` to those PRs and close them. That, obviously, is not a great outcome for us. And it's especially bad for the contributor, as they've spent a lot of effort preparing the change.
  To resolve this problem, we've decided to separate a bucket of issues, which would be great candidates for community members to contribute to. We mark these issues with the `help wanted` label.

Within that set, we have additionally marked issues that are good candidates for first-time contributors. Those do not require too much familiarity with the framework and are more novice-friendly. Those are marked with the `good first issue` label.


If you would like to make a contribution to an area not documented here, first open an issue with a description of the change you would like to make and the problem it solves so it can be discussed before a pull request is submitted.

### Before writing code

  We've seen PRs, where customers would solve an issue in a way, which either wouldn't fit into the framework because of how it's designed or it would change the framework in a way, which is not something we'd like to do. To avoid these situations and potentially save you a lot of time, we encourage customers to discuss the preferred design with the team first. To do so, file a new `design proposal` issue, link to the issue you'd like to address, and provide detailed information about how you'd like to solve a specific problem. We triage issues periodically and it will not take long for a team member to engage with you on that proposal.
  When you get an agreement from our team members that the design proposal you have is solid, then go ahead and prepare the PR.
  To file a design proposal, look for the relevant issue in the `New issue` page.

### Before submitting the pull request

Before submitting a pull request, make sure that it checks the following requirements:

* You find an existing issue with the "help-wanted" label or discuss with the team to agree on adding a new issue with that label
* You post a high-level description of how it will be implemented and receive a positive acknowledgement from the team before getting too committed to the approach or investing too much effort in implementing it.
* You add test coverage following existing patterns within the codebase
* Your code matches the existing syntax conventions within the codebase
* Your PR is small, focused, and avoids making unrelated changes

If your pull request contains any of the below, it's less likely to be merged.

* Changes that break backward compatibility
* Changes that are only wanted by one person/company. Changes need to benefit a large enough proportion of Scrapy.NET developers.
* Changes that add entirely new feature areas without prior agreement
* Changes that are mostly about refactoring existing code or code style
* Very large PRs that would take hours to review (remember, we're trying to help lots of people at once). For larger work areas, please discuss with us to find ways of breaking it down into smaller, incremental pieces that can go into separate PRs.

### During pull request review

A core contributor will review your pull request and provide feedback. To ensure that there is not a large backlog of inactive PRs, the pull request will be marked as stale after two weeks of no activity. After another four days, it will be closed.

### Resources to help you get started

Here are some resources to help you get started on how to contribute code or new content.

* Look at the [Contributor documentation](/docs/README.md) to get started on building the source code on your own.
* ["Help wanted" issues](https://github.com/malisancube/scrapynetdev/labels/help%20wanted) - these issues are up for grabs. Comment on an issue if you want to create a fix.
* ["Good first issue" issues](https://github.com/malisancube/scrapynetdev/labels/good%20first%20issue) - we think these are good for newcomers.

### Submitting a pull request

If you don't know what a pull request is read this article: <https://help.github.com/articles/using-pull-requests>. Make sure the repository can build and all tests pass. Familiarize yourself with the project workflow and our coding conventions. The coding, style, and general engineering guidelines are published on the [Engineering guidelines](https://github.com/malisancube/scrapynetdev/wiki/Engineering-guidelines) page.

### Tests

* Tests need to be provided for every bug/feature that is completed.
* Tests only need to be present for issues that need to be verified by QA (for example, not tasks)
* If there is a scenario that is far too hard to test there does not need to be a test for it.
  * "Too hard" is determined by the team as a whole.

### Feedback

Your pull request will now go through extensive checks by the subject matter experts on our team. Please be patient; we have hundreds of pull requests across all of our repositories. Update your pull request according to feedback until it is approved by one of the ASP.NET team members. After that, one of our team members may adjust the branch you merge into based on the expected release schedule.

## Code of conduct

See [CODE-OF-CONDUCT.md](./CODE-OF-CONDUCT.md)
