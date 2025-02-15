import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslationService } from '../../../services/translation.service';
import { MatDialog } from '@angular/material/dialog';
import { BlogService, Post } from './blog.service';
import { BlogPostDialogComponent } from './blog-post-dialog/blog-post-dialog.component';

@Component({
  selector: 'app-blog',
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
})
export class BlogComponent implements OnInit {
  title: string = '';
  posts: Post[] = [];

  constructor(
    private translationService: TranslationService,
    private blogService: BlogService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.title = this.translationService.instant('COMPONENTS.BLOG.TITLE');
    this.loadPosts();
  }

  loadPosts(): void {
    this.blogService.getPosts().subscribe(
      (data: Post[]) => {
        this.posts = data;
      },
      (error) => {
        console.error('Erro ao carregar as publicações:', error);
      }
    );
  }

  openPostDialog(post: Post): void {
    this.dialog.open(BlogPostDialogComponent, {
      width: '90%',
      maxWidth: '800px',
      maxHeight: '80vh',
      data: post,
      panelClass: 'custom-dialog-container'
    });
  }
}
