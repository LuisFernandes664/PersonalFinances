import { Component, ViewEncapsulation } from '@angular/core';
import { IFormField } from '../shared/form-field.interface';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {

  fields: IFormField[] = [
    {
      name: 'username',
      type: 'text',
      placeholder: 'COMPONENTS.AUTH.SIGNUP.USERNAME',
      required: true,
      pattern: '',
      value: '',
      touched: false
    },
    {
      name: 'email',
      type: 'email',
      placeholder: 'COMPONENTS.AUTH.SIGNUP.EMAIL',
      required: true,
      pattern: '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}',
      value: '',
      touched: false
    },
    {
      name: 'password',
      type: 'password',
      placeholder: 'COMPONENTS.AUTH.SIGNUP.PASSWORD',
      required: true,
      pattern: '',
      value: '',
      touched: false
    },
    {
      name: 'confirmPassword',
      type: 'password',
      placeholder: 'COMPONENTS.AUTH.SIGNUP.CONFIRM_PASSWORD',
      required: true,
      pattern: '',
      value: '',
      touched: false
    }
  ]
}
