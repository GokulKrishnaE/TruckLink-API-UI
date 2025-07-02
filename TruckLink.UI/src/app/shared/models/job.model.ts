export interface JobModel{
    id?:number|undefined,
    loadItem: string,
    Desciption:string,
    ContactInfo:string,
    startLocation: string,
    destination: string,
    earnings: number,
    distanceKm: number,
    mapUrl: string
    jobId?:number
}