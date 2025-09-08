export interface Cascade<T> extends Record<string, unknown> {
  value: T
  label: string
  children?: Cascade<T>[]
  disableCheckbox?: boolean
}
