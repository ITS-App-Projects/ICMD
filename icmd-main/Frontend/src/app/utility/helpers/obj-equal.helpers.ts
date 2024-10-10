export const objEqualWithJson = <T>(a: T, b: T): boolean => JSON.stringify(a) === JSON.stringify(b);

export const objEqualWithEcxeptions = <T>(a: T, b: T, exceptFields: ReadonlyArray<string> = []): boolean =>
  Object.keys(a)
    .filter((key) => exceptFields.findIndex((ef) => ef === key) === -1)
    .findIndex((key) => a[key] !== b[key]) === -1;