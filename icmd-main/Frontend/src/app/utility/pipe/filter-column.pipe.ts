import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'filterColumns',
    standalone: true
})
export class FilterColumnsPipe implements PipeTransform {
    transform(displayedColumns: string[]): string[] {
        return displayedColumns.map(x => `${x}_Filter`);
    }
}