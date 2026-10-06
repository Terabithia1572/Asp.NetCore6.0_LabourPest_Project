// Build-only tool: originals and database paths are never changed.
import fs from 'node:fs/promises';
import path from 'node:path';
import {fileURLToPath} from 'node:url';
import {createHash} from 'node:crypto';
import sharp from 'sharp';

const root = fileURLToPath(new URL('../../Asp.NetCore6.0_LabourPest_Project/wwwroot/', import.meta.url));
const output = path.join(root, 'public-web/images/responsive');
await fs.mkdir(output, {recursive: true});
const sources = ['canabicom/images/pestcontrol.webp', 'canabicom/images/labourpesticon2.webp'];
for (const directory of ['labourpestcustomer/homePageImage', 'labourpestcustomer/servicesImage']) {
    for (const entry of await fs.readdir(path.join(root, directory), {withFileTypes: true}))
        if (entry.isFile() && /\.(webp|png|jpe?g)$/i.test(entry.name)) sources.push(directory + '/' + entry.name);
}
const manifest = {};
let before = 0, after = 0;
for (const source of sources) {
    const bytes = await fs.readFile(path.join(root, source));
    const hash = createHash('sha256').update(bytes).digest('hex');
    const metadata = await sharp(bytes).metadata();
    const maximum = source.includes('labourpesticon') ? 96 : Math.min(1280, metadata.width);
    const widths = [...new Set([384, 640, 960, maximum].filter(w => w <= maximum))].sort((a,b) => a-b);
    const variants = [];
    for (const width of widths) {
        const name = hash.slice(0,24) + '-' + width + '.webp';
        const result = await sharp(bytes).rotate().resize({width, withoutEnlargement: true}).webp({quality: 82, effort: 6}).toBuffer();
        await fs.writeFile(path.join(output, name), result);
        variants.push({width, url: '/public-web/images/responsive/' + name});
        if (width === maximum) after += result.length;
    }
    manifest['/' + source] = {sha256: hash, variants};
    before += bytes.length;
}
await fs.writeFile(path.join(output, 'manifest.json'), JSON.stringify(manifest, null, 2) + '\n');
console.log(JSON.stringify({images: sources.length, originalBytes: before, largestDerivativeBytes: after}));
