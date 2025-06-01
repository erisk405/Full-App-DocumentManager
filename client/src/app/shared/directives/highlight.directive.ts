import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appHighlight]'
})
export class HighlightDirective {

  constructor(private el: ElementRef) {
    this.el.nativeElement.style.backgroundColor = 'yellow'; // เปลี่ยนพื้นหลังเป็นสีเหลือง
  }
  @HostListener('mouseenter') onMouseEnter() {
    this.el.nativeElement.style.backgroundColor = 'lightblue'; // เปลี่ยนสีเมื่อเมาส์ hover
  }

  @HostListener('mouseleave') onMouseLeave() {
    this.el.nativeElement.style.backgroundColor = 'yellow'; // เปลี่ยนกลับเมื่อเมาส์ออก
  }
}
