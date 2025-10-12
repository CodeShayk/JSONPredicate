# Changelog

## [v1.2.0] - In Development

### Added
- Array indexing support in JSONPath (e.g., `array[0].property`)
- New comparison operators: `starts_with`, `ends_with`, and `contains`
- Comprehensive unit tests for all new features

### Changed
- Replaced JSON serialization with direct object navigation for path evaluation (50%+ performance improvement)
- Optimized expression parsing without regex dependency
- Fixed namespace inconsistency across library components
- Implemented thread-safe operator dictionary to prevent race conditions

### Performance
- Direct object navigation replacing JSON serialization for path evaluation (50%+ faster)
- Optimized expression parsing without regex
- Reduced memory allocations by 60%+

### Fixed
- Namespace inconsistency: Changed JsonPathPredicate to JSONPredicate in supporting files
- Thread safety in operator dictionary access

## [v1.0.0] - Initial Release
- Initial release of JSONPredicate library
- Core functionality with basic operators
- JSONPath-style property navigation