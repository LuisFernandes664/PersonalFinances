import { Component, Input, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../auth.service';
import { IFormField } from '../form-field.interface';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService } from '../../../../shared/notifications/notification.service';
import { TranslationService } from '../../../../../services/translation.service';
import { IUser } from '../../../../../models/user.model';

@Component({
  selector: 'app-auth-form',
  templateUrl: './auth-form.component.html',
  styleUrls: ['./auth-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AuthFormComponent {
  @Input() title: string = this.translate.instant('COMPONENTS.AUTH.SIGNIN');
  @Input() buttonText: string = this.translate.instant('COMPONENTS.AUTH.SIGNIN');
  @Input() fields: IFormField[] = [];
  @Input() mode: 'login' | 'signup' = 'login';

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private translate: TranslationService,
  ) {}

  onFieldTouch(field: IFormField): void {
    field.touched = true;
    this.validateField(field);
  }

  validateField(field: IFormField): boolean {
    field.errorMessage = '';

    if (field.required && !field.value.trim()) {
      field.errorMessage = this.translate.instant('COMPONENTS.AUTH.REQUIRED_FIELD', { field: field.placeholder });
      return false;
    }

    if (field.pattern) {
      try {
        const regex = new RegExp(field.pattern);
        if (!regex.test(field.value.trim())) {
          field.errorMessage = this.translate.instant('COMPONENTS.AUTH.INVALID_FORMAT', { field: field.placeholder });
          return false;
        }
      } catch (error) {
        console.error(`Erro ao compilar o regex para ${field.placeholder}:`, error);
      }
    }

    return true;
  }

  onSubmit(): void {
    let formData: any = {};
    let isValid = true;

    this.fields.forEach(field => {
      field.touched = true;
      if (!this.validateField(field)) {
        isValid = false;
      }
      formData[field.name] = field.value.trim();
    });

    if (!isValid) {
      this.notificationService.showToast('warning', this.translate.instant('COMPONENTS.AUTH.FIX_ERRORS'));
      return;
    }

    if (this.mode === 'signup') {
      const user: IUser = {
        username: formData['username'] || '',
        email: formData['email'] || '',
        password: formData['password'] || '',
        phoneNumber: formData['phoneNumber']
      };
      this.signUp(user);
    } else {
      const credentials = {
        email: formData['email'] || '',
        password: formData['password'] || ''
      };

      this.signIn(credentials);
    }
  }

  signUp(formData: IUser): void {
    this.authService.signUp(formData).subscribe(
      res => {
        if (res.success && res.data) {
          const credentials = {
            email: formData.email,
            password: formData.password
          };
          this.signIn(credentials);
        } else {
          this.notificationService.showToast('error', res.message);
        }
      },
      err => {
        this.notificationService.showToast('error', err.message);
      }
    );
  }

  signIn(formData: any): void {
    this.authService.signIn(formData).subscribe();
  }
}
