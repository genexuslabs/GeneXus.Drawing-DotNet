# Contribution
Contribution is encouraged so long as they adhere to our guidelines and complies with Apache 2.0 licence.

This repository uses _topic branches_ for organizing contributions always starting from the `main` branch. While there are several other branches in this repository (e.g. `release/*`), contributions will only be accepted on the `main` branch. Please refer to [Branches](#branches) section for detailed information.

To be considered for inclusion, a contribution must fall into one of the following categories:

1. **Issue Report** (`issue/*`):<br>
   This includes bug fixes or code enhancements. Provide a clear description of the issue and the proposed solution.

2. **Feature Implementation** (`feature/*`):<br>
   Propose a new feature and explain its importance and relevance to the project. A feature may address the completion of functionality that is currently only partially supported.

Each contribution must be associated with an [Incident Report](#incident-report), which should include a detailed description of the problem and, where applicable, a brief code snippet demonstrating the obtained result vs the expected result. Unit tests are preferred to validate the changes.

Last but not least, it is the responsibility of the reporter to review existing incidents before submitting a new report. This practice improves communication with other contributors and our team, helping to avoid duplicating issues or reporting incidents that have already been resolved.

## Incident Report
Incidents can be reported through the Issues tab in this repository and must be written in **English** to ensure clear communication and accessibility for all contributors.

The report should include a descriptive title and adhere to the following template:

> **Description:**<br>
> Provide a concise description of the issue, including details about the error and the expected behavior. If the issue involves graphical errors, attach a set of relevant images that help to visually understand the problem.
>
> **Sample Code:**<br>
> Include a brief piece of code that reliably reproduces the issue, indicating the obtained and expected results. This helps in diagnosing and addressing the problem efficiently.


Additionally, feel free to provide any other pertinent information that may assist in understanding or resolving the issue. This may include environment details, steps to reproduce, or any error messages encountered.

## Fork & Pull Requests
Contribution acceptance are made by following Fork & Pull Request (PR) provided by Github. If you are getting started in Github, check [how to create a Fork](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-forks) and [work with a Pull Request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request-from-a-fork) articles.

### Process
The contribution process involves five steps:

1. **Fork the repository**:<br>
   Create your own copy of the project repository.

2. **Implement your changes**:<br>
   Develop and make the necessary modifications in your forked repository.

3. **Write unit tests**:<br>
   Create unit tests using `NUnit` framework to verify your implementation and ensure it functions as expected. Submitting unit tests is mandatory for approval.

4. **Verify your code**:<br>
   Ensure your code compiles without errors or warnings and passes all unit tests, including both those you have written and the existing ones.

5. **Open a new Pull Request**:<br>
   Submit a new Pull Request detailing the changes you have made.

Your Pull Request will undergo a review process by the repository managers within the organization before it can be merged into the `main` branch. 

The possible outcomes of a Pull Request review could be:

* **Approved**:<br>
   The Pull Request is accepted and merged into the `main` branch.

* **Requires Improvements**:<br>
   The Pull Request needs further modifications before it can be merged.

* **Rejected**:<br>
   The Pull Request is not accepted and will not be merged.

Only Pull Requests that receive an **Approved** status will be merged into the `main` branch for inclusion in future releases.

# Resources
A collection of resources that may be helpful for implementing `GeneXus.Drawing` functionalities to replicate the behavior of `System.Drawing`.

| Resource | Description
|----------|------------------------------------------
| [Microsoft: System.Drawing namespace](https://learn.microsoft.com/en-us/dotnet/api/system.drawing) | Documentation for `System.Drawing` library
| [Microsoft: SkiaSharp namespace](https://learn.microsoft.com/en-us/dotnet/api/skiasharp) | Documentation for `SkiaSharp` library
| [Skia: Documentation](https://api.skia.org/) | Documentation for `Skia` library.
| [Github: System.Drawing.Common](https://github.com/dotnet/winforms/tree/8dd0293125e7be7d7215bfdca6c5adc7286d7859/src/System.Drawing.Common/src/System/Drawing) | Source code for `System.Drawing` library.
| [Github: GDI+](https://github.com/mono/libgdiplus/tree/main/src) | Source code for `GDI+` library.
| [Github: SkiaSharp](https://github.com/mono/SkiaSharp/tree/main/binding/SkiaSharp) | Source code for `SkiaSharp` library.
| [NUnit: API Reference](https://docs.nunit.org/) | Documentation for `NUnit` framework.

# Branches
The following well-defined branches can be found within a repository.
| Name          | Description
|---------------|-------------
| `main`			| Stable development version of this library.
| `release/*`	| A set of stable versions of this library already released.
| `issue/*`		| A set of topic branches aimed to fix bugs in this library.
| `feature/*`	| A set of topic branches aimed to include features in this library.