import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export interface Post {
  id: number;
  title: string;
  summary: string;
  content: string;
}

@Injectable({
  providedIn: 'root'
})
export class BlogService {
  private posts: Post[] = [
    {
      id: 1,
      title: 'Título do Post 1',
      summary: 'Resumo do post 1. Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.',
      content: 'Conteúdo completo do post 1...'
    },
    {
      id: 2,
      title: 'Título do Post 2',
      summary: 'Resumo do post 2. Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.',
      content: 'Conteúdo completo do post 2...'
    },
    {
      id: 3,
      title: 'Título do Post 3',
      summary: 'Resumo do post 1. Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.',
      content: 'Conteúdo completo do post 1...'
    },
    {
      id: 4,
      title: 'Título do Post 4',
      summary: 'Resumo do post 2. Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.',
      content: 'Conteúdo completo do post 2...'
    },
    {
      id: 5,
      title: 'Título do Post 5',
      summary: 'Resumo do post 1. Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.',
      content: 'Conteúdo completo do post 1...Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.' +
      'Loremm ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl. Sed euismod, nunc vel tincidunt lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl eget nisl.'
    }
  ];

  getPosts(): Observable<Post[]> {
    return of(this.posts);
  }
}
