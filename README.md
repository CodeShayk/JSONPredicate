# <img src="https://github.com/CodeShayk/JSONPredicate/blob/master/Images/ninja-icon-16.png" alt="ninja" style="width:30px;"/> JSONPredicate v1.0.0
[![NuGet version](https://badge.fury.io/nu/JSONPredicate.svg)](https://badge.fury.io/nu/JSONPredicate) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/JSONPredicate/blob/master/LICENSE.md) 
[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/JSONPredicate?logo=github&sort=semver)](https://github.com/CodeShayk/JSONPredicate/releases/latest)
[![master-build](https://github.com/CodeShayk/JSONPredicate/actions/workflows/Master-Build.yml/badge.svg)](https://github.com/CodeShayk/JSONPredicate/actions/workflows/Master-Build.yml)
[![master-codeql](https://github.com/CodeShayk/JSONPredicate/actions/workflows/Master-CodeQL.yml/badge.svg)](https://github.com/CodeShayk/JSONPredicate/actions/workflows/Master-CodeQL.yml)
[![.Net 9.0](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
[![.Net Framework 4.6.4](https://img.shields.io/badge/.Net-4.6.2-blue)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net46)
[![.Net Standard 2.0](https://img.shields.io/badge/.NetStandard-2.0-blue)](https://github.com/dotnet/standard/blob/v2.0.0/docs/versions/netstandard2.0.md)

## What is JSONPredicate?

.Net library to provide a powerful and intuitive way to evaluate string-based predicate expressions against JSON objects using JSONPath syntax.

## Features

- **JSONPath Support**: Access `nested` object properties using `dot` notation
- **Multiple Operators**: `eq` (equal), `in` (contains), `not` (not equal), `gt` (greater than), `gte` (greater than or equal), `lt` (less than), `lte` (less than or equal)
- **Logical Operators**: `and`, `or` with proper precedence handling
- **Type Safety**: `Automatic` type conversion and validation
- **Complex Expressions**: `Parentheses` grouping and `nested` operations
- **Lightweight**: `Minimal` dependencies, `fast` evaluation

## Installation

Install via NuGet Package Manager:
```bash
Install-Package JsonPredicate
```
## Getting Started
The library provides a powerful, flexible way to evaluate complex conditional logic against JSON objects, making it ideal for business rules, filtering, validation, and many other use cases in .NET applications.
#### i. Expression Syntax
The expression syntax is ([JSONPath] [Comparison Operator] [Value]) [Logical Operator] ([JSONPath] [Comparison Operator] [Value])
#### ii. Supported Operators
- Comparison Operators - `eq`, `in`, `gt`, `gte`, `lt`, `lte` & `Not`
- Logical Operators - `and` & `or`
### Example
```
var customer = new {
    client = new {
        address = new {
            postcode = "e113et",
            number = 123,
            active = true
        },
        tags = new[] { "vip", "premium" }
    },
    score = 95.5
};
```
#### i. Simple equality
```
bool result1 = JSONPredicate.Evaluate("client.address.postcode eq `e113et`", customer);
```
#### ii. Logical operators
```
bool result2 = JSONPredicate.Evaluate("client.address.postcode eq `e113et` and client.address.number eq 123", customer);
```
#### iii. Array operations
```
bool result3 = JSONPredicate.Evaluate("client.tags in [`vip`, `standard`]", customer);
```
## Developer Guide
Please see [Developer Guide](https://github.com/CodeShayk/JSONPredicate/wiki) for comprehensive documentation to integrate JSONPredicate in your project.

## License
This project is licensed under the `MIT License` - see the [license](LICENSE) file for details.

## Contributing
We welcome contributions! Please see our Contributing Guide for details.
- 📖 Read the [Documentation](https://github.com/CodeShayk/JSONPredicate/wiki)
- 🐛 If you are having problems, please let us know by raising a new issue [here](https://github.com/CodeShayk/JSONPredicate/issues/new/choose).
- 💬 Ask questions on [Discussions](https://github.com/CodeShayk/JSONPredicate/discussions)
- 💻 Code - Submit [pull](https://github.com/CodeShayk/JSONPredicate/pulls) requests

## Credits
Thank you for reading. Please fork, explore, contribute and report. Happy Coding !! :)
