import { Component } from '@angular/core';

import { MINISTERIOS_MOCK } from '../../shared/data/ministerios.mock';
import { MinisterioCard } from '../../shared/ui/ministerio-card/ministerio-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-ministerios',
  imports: [MinisterioCard, SectionHeading],
  templateUrl: './ministerios.html'
})
export class Ministerios {
  protected readonly ministerios = MINISTERIOS_MOCK;
}
