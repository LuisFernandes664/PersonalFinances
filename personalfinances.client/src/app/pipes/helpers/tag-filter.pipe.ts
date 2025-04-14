import { Pipe, PipeTransform } from '@angular/core';
import { Tag } from '../../components/core/home/tags/tag.model';

@Pipe({
  name: 'tagFilter'
})
export class TagFilterPipe implements PipeTransform {
  transform(tags: Tag[], searchTerm: string): Tag[] {
    if (!tags || !searchTerm) {
      return tags;
    }

    searchTerm = searchTerm.toLowerCase();

    return tags.filter(tag =>
      tag.name.toLowerCase().includes(searchTerm)
    );
  }
}
