import { ElementRef } from '@angular/core';
import { HighlightDirective } from './highlight.directive';

describe('HighlightDirective', () => {
  it('should create an instance', () => {
    const mockElementRef = { nativeElement: { style: {} } } as ElementRef;
    const directive = new HighlightDirective(mockElementRef);
    expect(directive).toBeTruthy();
  });
});
