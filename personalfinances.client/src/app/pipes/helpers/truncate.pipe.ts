import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {
  transform(text: string, limit: number = 100, ellipsis: string = '...'): string {
    if (!text) return '';
    return text.length > limit ? text.substring(0, limit) + ellipsis : text;
  }
}
