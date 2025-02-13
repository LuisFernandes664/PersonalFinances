import { Component, OnInit } from '@angular/core';
import { TranslationService } from '../../../services/translation.service';

@Component({
  selector: 'app-blog',
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.scss']
})
export class BlogComponent implements OnInit {
  title: string = '';

  constructor(private translationService: TranslationService) { }

  ngOnInit(): void {
    this.title = this.translationService.instant('COMPONENTS.BLOG.TITLE');
  }
}
