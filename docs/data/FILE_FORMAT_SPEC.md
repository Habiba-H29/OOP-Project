# File Format Specification

## Overview

The persistence file `products.txt` stores one product per line in pipe-delimited format.

- Delimiter: `|`
- Record types:
  - `P` for perishable products
  - `N` for non-perishable products

## Record Definitions

## Perishable Record

```text
P|ProductID|Name|Category|Price|Quantity|ExpiryDate|StorageTemp
```

Field order:
1. RecordType: literal `P`
2. ProductID: integer
3. Name: string (no delimiter escaping currently)
4. Category: enum name (`Dairy`, `Bakery`, etc.)
5. Price: floating-point numeric string
6. Quantity: integer
7. ExpiryDate: date string (currently short-date style)
8. StorageTemp: numeric string

## Non-Perishable Record

```text
N|ProductID|Name|Category|Price|Quantity|ShelfLifeMonths|Barcode
```

Field order:
1. RecordType: literal `N`
2. ProductID: integer
3. Name: string (no delimiter escaping currently)
4. Category: enum name
5. Price: floating-point numeric string
6. Quantity: integer
7. ShelfLifeMonths: integer
8. Barcode: string

## Examples

```text
P|1|Milk|Dairy|3.5|10|5/15/2026|4
N|2|Pasta|Bakery|2.99|25|24|1234567890
```

## Parsing Rules (Current Implementation)

- Split line by `|`.
- Skip line if field count is less than 8.
- Parse based on first token (`P` or `N`).
- Convert category via `Enum.Parse<Category>(...)`.
- Convert numerics via `int.Parse(...)` / `double.Parse(...)`.

## Constraints and Caveats

- No escaping mechanism for `|` inside text fields.
- Date and decimal parsing are culture-sensitive in current implementation.
- No explicit schema version in each record.
