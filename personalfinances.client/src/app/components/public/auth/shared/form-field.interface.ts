export interface IFormField {
  name: string;
  type: string;
  placeholder: string;
  required: boolean;
  pattern?: string;
  value: string;
  errorMessage?: string;
  touched: boolean;
}
