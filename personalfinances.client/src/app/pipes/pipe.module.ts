import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateFormatPipe } from './helpers/date-format.pipe';
import { TruncatePipe } from './helpers/truncate.pipe';
import { TagFilterPipe } from './helpers/tag-filter.pipe';

@NgModule({
  declarations: [
    TruncatePipe,
    DateFormatPipe,
    TagFilterPipe
  ],
  imports: [CommonModule],
  exports: [
    DateFormatPipe,
    TruncatePipe,
    TagFilterPipe
  ]
})
export class PipeModule {}
