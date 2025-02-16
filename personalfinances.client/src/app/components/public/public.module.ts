import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { ContactComponent } from './contact/contact.component';
import { HeaderPublicComponent } from './shared/header/header.component';
import { BrowserModule } from '@angular/platform-browser';
import { BlogComponent } from './blog/blog.component';
import { AuthModule } from './auth/auth.module';
import { TruncatePipe } from '../../pipes/truncate.pipe';
import { BlogPostDialogComponent } from './blog/blog-post-dialog/blog-post-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { PipeModule } from '../../pipes/pipe.module';


@NgModule({
  declarations: [
    ContactComponent,
    BlogComponent,
    BlogComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    BrowserModule,
    TranslateModule.forChild(),
    RouterModule,
    CommonModule,
    AuthModule,
    MatDialogModule,
    PipeModule
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  exports: [
    ContactComponent,
    BlogComponent
  ]
})
export class PublicModule { }
