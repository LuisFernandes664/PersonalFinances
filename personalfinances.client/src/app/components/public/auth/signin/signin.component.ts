import { Component, OnInit } from '@angular/core';
import { IFormField } from '../shared/form-field.interface';
import { TranslationService } from '../../../../services/translation.service';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.scss']
})
export class SigninComponent implements OnInit {
  fields: IFormField[] = [
    {
      name: 'email',
      type: 'email',
      placeholder: 'Email',
      required: true,
      pattern: '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}',
      value: '',
      touched: false
    },
    {
      name: 'password',
      type: 'password',
      placeholder: "Password",
      required: true,
      pattern: '',
      value: '',
      touched: false
    }
  ];

  title: string = '';
  buttonText: string = '';
  constructor(private translationService: TranslationService) {}


  ngOnInit(): void {
    this.title = this.translationService.instant('COMPONENTS.AUTH.SIGNIN');
    this.buttonText = this.translationService.instant('COMPONENTS.AUTH.SIGNIN');

    // Se desejar traduzir placeholders dinamicamente, pode usar:
    this.fields = this.fields.map(field => ({
      ...field,
      placeholder: this.translationService.instant(field.placeholder)
    }));
  }
}
