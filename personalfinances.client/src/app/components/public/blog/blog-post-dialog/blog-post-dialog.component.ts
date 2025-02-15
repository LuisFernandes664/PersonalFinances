import { Component, Inject, ViewEncapsulation } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Post } from '../blog.service';

@Component({
  selector: 'app-blog-post-dialog',
  templateUrl: './blog-post-dialog.component.html',
  styleUrls: ['./blog-post-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BlogPostDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<BlogPostDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Post
  ) {}

  fechar(): void {
    this.dialogRef.close();
  }
}
