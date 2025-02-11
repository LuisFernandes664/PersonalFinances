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
      placeholder: 'Nome de Utilizador',
      required: true,
      pattern: '',
      value: '',
      touched: false
    },
    {
      name: 'email',
      type: 'email',
      placeholder: 'Email',
      required: true,
      pattern: '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$',
      value: '',
      touched: false
    },
    {
      name: 'password',
      type: 'password',
      placeholder: 'Palavra-Passe',
      required: true,
      pattern: '',
      value: '',
      touched: false
    },
    {
      name: 'confirmPassword',
      type: 'password',
      placeholder: 'Confirmar Palavra-Passe',
      required: true,
      pattern: '',
      value: '',
      touched: false
    }
  ]
}
